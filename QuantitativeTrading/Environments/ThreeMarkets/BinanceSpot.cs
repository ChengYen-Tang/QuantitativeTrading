using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Spot;
using Binance.Net.Objects.Spot.SpotData;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Configuration;
using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records.ThreeMarkets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuantitativeTrading.Environments.ThreeMarkets
{
    public class BinanceSpot : IThreeMarketEnvironment, IDisposable
    {
        private readonly BinanceClient client;
        private readonly BinanceSocketClient socketClient;
        private readonly ContinuousResetEvent continuousResetEvent;
        private readonly string[] coinNames;
        private readonly string[] symbols;
        private readonly ThreeMarketsDataProviderModel dataProvider;

        private bool disposedValue;
        private Dictionary<string, BinanceBalance> balances;

        public decimal Assets => Balance + CoinBalance1 * dataProvider.Coin12CoinKline.Close + CoinBalance2 * dataProvider.Coin22CoinKline.Close;
        /// <summary>
        /// 餘額
        /// </summary>
        public decimal Balance { get { return balances[coinNames[0]].Total; } }
        /// <summary>
        /// Coin1 的餘額
        /// </summary>
        public decimal CoinBalance1 { get { return balances[coinNames[1]].Total; } }
        /// <summary>
        /// Coin2 的餘額
        /// </summary>
        public decimal CoinBalance2 { get { return balances[coinNames[2]].Total; } }

        public BinanceSpot(IConfiguration configuration, string baseCoin, string coin1, string coin2)
        {
            (socketClient, continuousResetEvent, coinNames, symbols, dataProvider) = (new(), new(45, 1000), new[] { baseCoin, coin1, coin2 }, new[] { $"{coin1}{baseCoin}", $"{coin2}{baseCoin}", $"{coin2}{coin1}" }, new());
            string key = configuration["Key"];
            string secret = configuration["Secret"];
            client = new(new BinanceClientOptions() { ApiCredentials = new(key, secret) });
        }

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

        public void Trading(TradingAction action, TradingMarket market)
            => client.Spot.Order.PlaceOrder(MarketToSymbol(market), ActionToOrderSide(action), OrderType.Market);

        /// <summary>
        /// 紀錄資料
        /// </summary>
        /// <param name="record"></param>
        public void Recording(Models.Records.IEnvironmentModels record)
        {
            IEnvironmentModels spotRecord = record as IEnvironmentModels;
            spotRecord.Assets = Assets;
            spotRecord.Balance = Balance;
            spotRecord.CoinBalance1 = CoinBalance1;
            spotRecord.CoinBalance2 = CoinBalance2;
            spotRecord.Date = dataProvider.Coin12CoinKline.Date;
            spotRecord.Coin12CoinClose = dataProvider.Coin12CoinKline.Close;
            spotRecord.Coin22CoinClose = dataProvider.Coin22CoinKline.Close;
            spotRecord.Coin22Coin1Close = dataProvider.Coin22Coin1Kline.Close;
        }

        public async Task<bool> ReflashAcountInfo()
        {
            Task cancelOrder1 = client.Spot.Order.CancelAllOpenOrdersAsync(symbols[0]);
            Task cancelOrder2 = client.Spot.Order.CancelAllOpenOrdersAsync(symbols[1]);
            Task cancelOrder3 = client.Spot.Order.CancelAllOpenOrdersAsync(symbols[2]);
            Task.WaitAll(cancelOrder1, cancelOrder2, cancelOrder3);
            WebCallResult<BinanceAccountInfo> result = await client.General.GetAccountInfoAsync();
            balances = result.Data.Balances.ToDictionary(item => item.Asset);
            return result.Success;
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
