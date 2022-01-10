using MultilateralArbitrage.Models;
using MultilateralArbitrage.Modules;
using MultilateralArbitrage.Modules.API;
using MultilateralArbitrage.Modules.RevenusSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultilateralArbitrage
{
    public class Program
    {
        private const string startAsset = "USDT";

        public static async Task Main()
        {
            // Download spots symbols from binance and calculate market mix
            IAPI api = new Modules.API.Binance();
            ICollection<Symbol> symbols = await api.DownloadSymbolsAsync();
            IDictionary<string, ICollection<Symbol>> classificationSymbols = symbols.ToClassificationSymbols();
            MarketMix marketMix = new(classificationSymbols, new string[] { "NGN" }, 5);
            ICollection<ICollection<Symbol>> allMarketMix = marketMix.GetAllMarketMix(startAsset);
            Console.WriteLine($"市場數量: {symbols.Count}");
            Console.WriteLine($"組合數量: {allMarketMix.Count}");
            // New revenus simulator instance
            Collision collision = new(allMarketMix, 0.1);
            CollisionAndLastStepPadding collisionAndLastStepPadding = new(allMarketMix, 0.1);
            while (true)
            {
                // Get data from binance
                DateTime nowTime = DateTime.Now;
                Task<IDictionary<string, OrderBook>> orderBooksTask = api.GetAllOrderBooksAsync();
                Task<IDictionary<string, LatestPrice>> latestPricesTask = api.GetAllLatestPrices();
                IDictionary<string, OrderBook> orderBooks = await orderBooksTask;
                IDictionary<string, LatestPrice> latestPrices = await latestPricesTask;
                if (orderBooks is null && latestPrices is null)
                    continue;

                // Calculate all income
                Task<ICollection<(ICollection<Symbol> marketMix, float assets)>> collisionTask = collision.CalculateAllIncomeAsync(startAsset, orderBooks!);
                Task<ICollection<(ICollection<Symbol> marketMix, float assets)>> collisionAndLastStepPaddingTask = collisionAndLastStepPadding.CalculateAllIncomeAsync(startAsset, orderBooks!, latestPrices!);
                IEnumerable<(ICollection<Symbol> marketMix, float assets)> collisionRevenus = (await collisionTask).Where(item => item.assets > 1);
                IEnumerable<(ICollection<Symbol> marketMix, float assets)> collisionAndLastStepPaddingRevenus = (await collisionAndLastStepPaddingTask).Where(item => item.assets > 1);
                // Save symbols to database while income > 1%
                using ApplicationDbContext db = new();
                db.CollisionAssetsRecords.AddRange(collisionRevenus.Select(item => new CollisionAssetsRecord() { Assets = item.assets, MarketMix = string.Join(", ", item.marketMix.Select(item => item.Name)) }));
                db.CollisionAndLastStepPaddingAssetsRecords.AddRange(collisionAndLastStepPaddingRevenus.Select(item => new CollisionAndLastStepPaddingAssetsRecord() { Assets = item.assets, MarketMix = string.Join(", ", item.marketMix.Select(item => item.Name)) }));
                await db.SaveChangesAsync();
                if (collisionRevenus.Any() || collisionAndLastStepPaddingRevenus.Any())
                    Console.WriteLine(DateTime.Now.ToString());
                await Task.Delay(25000);

                SpinWait.SpinUntil(() => DateTime.Now > nowTime.AddSeconds(30));
            }
        }
    }
}