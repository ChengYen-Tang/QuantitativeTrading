using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using QuantitativeTrading.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuantitativeTrading.Environments.ThreeMarkets
{
    public class BinanceSpot : IDisposable
    {
        private readonly BinanceClient client;
        private readonly BinanceSocketClient socketClient;
        private readonly ContinuousResetEvent continuousResetEvent;
        private readonly string[] symbols;
        private readonly ThreeMarketsDataProviderModel dataProvider;

        private bool disposedValue;

        public BinanceSpot(string[] symbols)
            => (socketClient, continuousResetEvent, this.symbols, dataProvider) = (new(), new(45, 1000), symbols, new());

        public void Run()
        {
            foreach (var item in symbols.Select((value, index) => new { value, index }))
                socketClient.Spot.SubscribeToKlineUpdates(item.value, KlineInterval.OneMinute, (data) =>
                {
                    if (data != null && data.Data.Final)
                    {
                        switch (item.index)
                        {
                            case 1:
                                dataProvider.Coin22CoinKline = ToKlineModel(data.Data);
                                break;
                            case 2:
                                dataProvider.Coin22Coin1Kline = ToKlineModel(data.Data);
                                break;
                            default:
                                dataProvider.Coin12CoinKline = ToKlineModel(data.Data);
                                break;
                        }
                        continuousResetEvent.Set();
                    }
                });
        }

        public async Task<ThreeMarketsDataProviderModel> GetKlineAsync()
        {
            await continuousResetEvent.WaitOneAsync();
            return new ThreeMarketsDataProviderModel(dataProvider);
        }

        public async Task TradingAsync(TradingAction action, TradingMarket market)
        {
            Task cancelOrder1 = client.Spot.Order.CancelAllOpenOrdersAsync(symbols[0]);
            Task cancelOrder2 = client.Spot.Order.CancelAllOpenOrdersAsync(symbols[1]);
            Task cancelOrder3 = client.Spot.Order.CancelAllOpenOrdersAsync(symbols[2]);
            await Task.WhenAll(cancelOrder1, cancelOrder2, cancelOrder3);
            await client.Spot.Order.PlaceOrderAsync(MarketToSymbol(market), ActionToOrderSide(action), OrderType.Market);
        }

        public async Task CloseAsync()
            => await socketClient.UnsubscribeAll();


        private static KlineModel ToKlineModel(IBinanceStreamKline data)
            => new() { Open = data.Open, Close = data.Close, Date = data.CloseTime, High = data.High, Low = data.Low, Money = data.QuoteVolume, Volume = data.BaseVolume, TakerBuyBaseVolume = data.TakerBuyBaseVolume, TakerBuyQuoteVolume = data.TakerBuyQuoteVolume, TradeCount = data.TradeCount };

        private string MarketToSymbol(TradingMarket market)
            => (market) switch
            {
                TradingMarket.Coin22Coin => symbols[1],
                TradingMarket.Coin22Coin1 => symbols[2],
                _ => symbols[0]
            };

        private static OrderSide ActionToOrderSide(TradingAction action)
            =>  action == TradingAction.Buy ? OrderSide.Buy : OrderSide.Sell;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                    socketClient.Dispose();
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        ~BinanceSpot()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
