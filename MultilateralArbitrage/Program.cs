using MultilateralArbitrage.Models;
using MultilateralArbitrage.Modules;
using MultilateralArbitrage.Modules.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultilateralArbitrage
{
    public class Program
    {
        private const string startAsset = "USDT";

        public static async Task Main()
        {

            IAPI api = new Modules.API.Binance();
            ICollection<Symbol> symbols = await api.DownloadSymbolsAsync();
            IDictionary<string, ICollection<Symbol>> classificationSymbols = symbols.ToClassificationSymbols();
            MarketMix marketMix = new(classificationSymbols, new string[] { "NGN" }, 5);
            ICollection<ICollection<Symbol>> allMarketMix = marketMix.GetAllMarketMix(startAsset);
            Console.WriteLine($"市場數量: {symbols.Count}");
            Console.WriteLine($"組合數量: {allMarketMix.Count}");
            Collision simulator = new(allMarketMix, 0.1);
            while (true)
            {
                IDictionary<string, OrderBook> orderBooks = await api.GetAllOrderBooksAsync();
                if (orderBooks is null)
                    continue;
                IEnumerable<(ICollection<Symbol> marketMix, float assets)> revenus = (await simulator.CalculateAllIncomeAsync(startAsset, orderBooks)).Where(item => item.assets > 1);
                using ApplicationDbContext db = new();
                db.AssetsRecords.AddRange(revenus.Select(item => new AssetsRecord() { Assets = item.assets, MarketMix = string.Join(", ", item.marketMix.Select(item => item.Name)) }));
                await db.SaveChangesAsync();
                if (revenus.Any())
                    Console.WriteLine(DateTime.Now.ToString());
                await Task.Delay(30000);
            }
        }
    }
}