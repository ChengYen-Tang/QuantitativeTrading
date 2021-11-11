using MultilateralArbitrage.Models;
using MultilateralArbitrage.Modules;
using MultilateralArbitrage.Modules.API;

namespace MultilateralArbitrage;

public class Program
{
    public static async Task Main(string[] args)
    {
        IAPI api = new Modules.API.Binance();
        IDictionary<string, ICollection<Symbol>> symbols = (await api.DownloadSymbolsAsync()).ToClassificationSymbol();
        MarketMix marketMix = new(symbols, 5);
        ICollection<ICollection<Symbol>> allMarketMix = marketMix.GetAllMarketMix("BUSD");
        Console.WriteLine(allMarketMix.Count);
        Console.ReadKey();
    }
}
