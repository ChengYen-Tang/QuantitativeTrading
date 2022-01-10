using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.SpotData;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using EasyCaching.Core;
using MultilateralArbitrage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultilateralArbitrage.Modules.API
{
    internal class Binance : IAPI, IDisposable
    {
        private readonly IEasyCachingProvider easyCaching;
        private readonly ApiCredentials credentials;
        private readonly BinanceClient client;
        private readonly BinanceSocketClient socketClient;
        private readonly UpdateSubscription updateSubscription;
        private bool disposedValue;

        public Binance()
            => (client) = (new());

        public Binance(string key, string secret, IEasyCachingProviderFactory easyCaching)
        {
            this.easyCaching = easyCaching.GetCachingProvider("default1");
            credentials = new(key, secret);
            client = new(new() { ApiCredentials = credentials });
            socketClient = new(new() { ApiCredentials = credentials });
            WebCallResult<string> keyResult = client.Spot.UserStream.StartUserStreamAsync().Result;
            if (keyResult.Success)
            {
                CallResult<UpdateSubscription>  updateSubscriptionResult = socketClient.Spot.SubscribeToUserDataUpdatesAsync(keyResult.Data, (order) =>
                {
                    while (!this.easyCaching.TrySet(order.Data.OrderId.ToString(), order.Data.IsWorking, TimeSpan.FromDays(1))) ;
                }, null, null, null).Result;
                if (updateSubscriptionResult.Success)
                {
                    updateSubscription = updateSubscriptionResult.Data;
                }
            }
                
        }

        public async Task<ICollection<Symbol>> DownloadSymbolsAsync()
        {
            WebCallResult<BinanceExchangeInfo> webCallResult = await client.Spot.System.GetExchangeInfoAsync();
            if (!webCallResult.Success)
                return null!;

            BinanceExchangeInfo binanceExchangeInfo = webCallResult.Data;
            return binanceExchangeInfo.Symbols.Select(binanceSymbol => new Symbol() { Name = binanceSymbol.Name, BaseAsset = binanceSymbol.BaseAsset, QuoteAsset = binanceSymbol.QuoteAsset }).ToArray();
        }

        public async Task<IDictionary<string, OrderBook>> GetAllOrderBooksAsync()
        {
            WebCallResult<IEnumerable<BinanceBookPrice>> webCallResult = await client.Spot.Market.GetAllBookPricesAsync();
            if (!webCallResult.Success)
                return null!;

            IEnumerable<BinanceBookPrice> bookPrice = webCallResult.Data;
            return bookPrice.ToDictionary(item => item.Symbol, item => new OrderBook(item.BestBidPrice, item.BestBidQuantity, item.BestAskPrice, item.BestAskQuantity, item.Timestamp));
        }

        public async Task<IDictionary<string, LatestPrice>> GetAllLatestPrices()
        {
            WebCallResult<IEnumerable<BinancePrice>> webCallResult = await client.Spot.Market.GetPricesAsync();
            if (!webCallResult.Success)
                return null!;

            IEnumerable<BinancePrice> prices = webCallResult.Data;
            return prices.ToDictionary(item => item.Symbol, item => new LatestPrice(item.Price, item.Timestamp));
        }

        public async Task MovingBricks(ICollection<Order> orders)
        {
            foreach (Order order in orders)
            {
                BinancePlacedOrder placedOrder = await PlaceOrder(order);
                SpinWait.SpinUntil(() => { var result = easyCaching.Get<bool>(placedOrder.OrderId.ToString()); return !result.Value; });
            }
        }

        private async Task<BinancePlacedOrder> PlaceOrder(Order order)
        {
            WebCallResult<BinancePlacedOrder> result;

            if (order.IsBuy)
            {
                decimal balance = await GetBalances(order.Symbol.QuoteAsset);
                result = order.IsImmediatelyMatch
                    ? await client.Spot.Order.PlaceOrderAsync(order.Symbol.Name, OrderSide.Buy, OrderType.Market, quoteOrderQuantity: balance)
                    : await client.Spot.Order.PlaceOrderAsync(order.Symbol.Name, OrderSide.Buy, OrderType.LimitMaker, quoteOrderQuantity: balance, price: order.Price);
            }
            else
            {
                decimal balance = await GetBalances(order.Symbol.BaseAsset);
                result = order.IsImmediatelyMatch
                    ? await client.Spot.Order.PlaceOrderAsync(order.Symbol.Name, OrderSide.Sell, OrderType.Market, balance)
                    : await client.Spot.Order.PlaceOrderAsync(order.Symbol.Name, OrderSide.Sell, OrderType.LimitMaker, balance, price: order.Price);
            }

            return result.Data;
        }

        private async Task<decimal> GetBalances(string asset)
        {
            WebCallResult<BinanceAccountInfo> result = await client.General.GetAccountInfoAsync();
            return result.Data.Balances.First(item => item.Asset == asset).Free;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                    updateSubscription.CloseAsync().Wait();
                    client?.Dispose();
                    socketClient?.Dispose();
                    credentials?.Dispose();
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        ~Binance()
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