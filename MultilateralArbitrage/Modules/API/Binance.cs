using Binance.Net;
using Binance.Net.Objects.Spot.MarketData;
using CryptoExchange.Net.Objects;
using MultilateralArbitrage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultilateralArbitrage.Modules.API
{
    internal class Binance : IAPI, IDisposable
    {
        private readonly BinanceClient client;
        private bool disposedValue;

        public Binance()
            => (client) = (new());

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

        public async Task<IDictionary<string, LatestPrice>> GetAllLatestPrice()
        {
            WebCallResult<IEnumerable<BinancePrice>> webCallResult = await client.Spot.Market.GetPricesAsync();
            if (!webCallResult.Success)
                return null!;

            IEnumerable<BinancePrice> prices = webCallResult.Data;
            return prices.ToDictionary(item => item.Symbol, item => new LatestPrice(item.Price, item.Timestamp));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                    client.Dispose();
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