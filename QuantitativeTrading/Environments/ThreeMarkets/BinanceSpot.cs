using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using QuantitativeTrading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuantitativeTrading.Environments.ThreeMarkets
{
    public class BinanceSpot : IDisposable
    {
        private readonly BinanceSocketClient socketClient;
        private readonly ContinuousResetEvent continuousResetEvent;
        private readonly IEnumerable<string> symbols;
        private readonly ThreeMarketsDataProviderModel dataProvider;

        private bool disposedValue;

        public BinanceSpot(IEnumerable<string> symbols)
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

        public async Task CloseAsync()
            => await socketClient.UnsubscribeAll();

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

        private static KlineModel ToKlineModel(IBinanceStreamKline data)
            => new() { Open = data.Open, Close = data.Close, Date = data.CloseTime, High = data.High, Low = data.Low, Money = data.QuoteVolume, Volume = data.BaseVolume, TakerBuyBaseVolume = data.TakerBuyBaseVolume, TakerBuyQuoteVolume = data.TakerBuyQuoteVolume, TradeCount = data.TradeCount };

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
