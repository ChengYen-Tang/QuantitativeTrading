// See https://aka.ms/new-console-template for more information
using Binance.Net;
using Binance.Net.Objects.Spot.MarketData;
using CryptoExchange.Net.Objects;

namespace MultilateralArbitrage;

public class Program
{
    public static async Task Main(string[] args)
    {
        BinanceClient client = new();
        WebCallResult<BinanceExchangeInfo> webCallResult = await client.Spot.System.GetExchangeInfoAsync();
        if (!webCallResult.Success)
        {
            Console.WriteLine(webCallResult.Error!.Message);
            return;
        }

        BinanceExchangeInfo binanceExchangeInfo = webCallResult.Data;

        Dictionary<string, List<BinanceSymbol>> symbols = new();
        foreach (BinanceSymbol symbol in binanceExchangeInfo.Symbols)
        {
            if (!symbols.ContainsKey(symbol.BaseAsset))
                symbols.Add(symbol.BaseAsset, new());
            if (!symbols.ContainsKey(symbol.QuoteAsset))
                symbols.Add(symbol.QuoteAsset, new());
            symbols[symbol.BaseAsset].Add(symbol);
            symbols[symbol.QuoteAsset].Add(symbol);
        }
        MarketMix marketMix = new(symbols, 5);
        ICollection<ICollection<BinanceSymbol>> allMarketMix = marketMix.GetAllMarketMix("BUSD");
        Console.WriteLine(allMarketMix.Count);
        Console.ReadKey();
    }
}

public class MarketMix
{
    private readonly Dictionary<string, List<BinanceSymbol>> symbols;
    private readonly int depth;
    private readonly object allMarketMixLock;

    public MarketMix(Dictionary<string, List<BinanceSymbol>> symbols, int depth = 0)
        => (this.symbols, this.depth, allMarketMixLock) = (symbols, depth, new());

    public ICollection<ICollection<BinanceSymbol>> GetAllMarketMix(string startAsset)
    {
        List<ICollection<BinanceSymbol>> allMarketMix = new();
        Ecplore(startAsset, startAsset, 0, new(), allMarketMix);
        return allMarketMix;
    }

    private void Ecplore(string startAsset, string ecploreAsset, int nowDepth, List<BinanceSymbol> marketMix, List<ICollection<BinanceSymbol>> allMarketMix)
    {
        if (depth is not 0 && nowDepth == depth)
            return;
        
        foreach (BinanceSymbol binanceSymbol in symbols[ecploreAsset])
        {
            if (marketMix.Contains(binanceSymbol))
                continue;

            List<BinanceSymbol> buffer = new(marketMix);
            buffer.Add(binanceSymbol);
            string asset = ecploreAsset == binanceSymbol.BaseAsset ? binanceSymbol.QuoteAsset : binanceSymbol.BaseAsset;
            if (asset == startAsset)
            {
                lock(allMarketMix)
                    allMarketMix.Add(buffer);
                continue;
            }

            Ecplore(startAsset, asset, nowDepth + 1, buffer, allMarketMix);
        }
    }
}
