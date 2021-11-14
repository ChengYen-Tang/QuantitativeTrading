using MultilateralArbitrage.Models;
using MultilateralArbitrage.Modules;
using MultilateralArbitrage.Modules.API;

namespace MultilateralArbitrage;

public class Program
{
    public static async Task Main(string[] args)
    {
        IAPI api = new Modules.API.Binance();
        ICollection<Symbol> symbols = await api.DownloadSymbolsAsync();
        IDictionary<string, OrderBook> orderBooks = await api.GetAllOrderBooksAsync();
        IDictionary<string, ICollection<Symbol>> classificationSymbols = symbols.ToClassificationSymbols();
        MarketMix marketMix = new(classificationSymbols, 5);
        ICollection<ICollection<Symbol>> allMarketMix = marketMix.GetAllMarketMix("BUSD");
        Console.WriteLine($"市場數量: {symbols.Count}");
        Console.WriteLine($"訂單書的市場數量: {orderBooks.Keys.Count}");
        Console.WriteLine($"組合數量: {allMarketMix.Count}");
        Console.ReadKey();
    }
}
