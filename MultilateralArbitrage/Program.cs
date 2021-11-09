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


        Console.WriteLine(string.Join('\n', binanceExchangeInfo.Symbols.Select(item => item.Name)));
        Console.WriteLine(binanceExchangeInfo.Symbols.Count());
        Console.ReadKey();
    }
}
