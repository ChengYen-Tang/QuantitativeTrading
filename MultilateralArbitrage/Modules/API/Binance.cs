using Binance.Net;
using Binance.Net.Objects.Spot.MarketData;
using CryptoExchange.Net.Objects;
using MultilateralArbitrage.Models;

namespace MultilateralArbitrage.Modules.API;
internal class Binance : IAPI
{
    private readonly BinanceClient client;

    public Binance()
        => client = new();

    public async Task<ICollection<Symbol>> DownloadSymbolsAsync()
    {
        WebCallResult<BinanceExchangeInfo> webCallResult = await client.Spot.System.GetExchangeInfoAsync();
        if (!webCallResult.Success)
            return null!;

        BinanceExchangeInfo binanceExchangeInfo = webCallResult.Data;
        return binanceExchangeInfo.Symbols.Select(binanceSymbol => new Symbol() { Name = binanceSymbol.Name, BaseAsset = binanceSymbol.BaseAsset, QuoteAsset = binanceSymbol.QuoteAsset }).ToArray();
    }
}
