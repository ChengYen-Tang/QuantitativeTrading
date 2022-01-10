using MultilateralArbitrage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultilateralArbitrage.Modules.RevenusSimulator
{
    /// <summary>
    /// 模擬收益的模組
    /// 前面交易皆強撞掛單，但最後一個交易市場是用最新成交價掛單
    /// </summary>
    internal class CollisionAndLastStepPadding
    {
        private readonly decimal fee;
        public ICollection<ICollection<Symbol>> AllMarketMix { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="allMarketMix"> 所有的投資組合 </param>
        /// <param name="fee"> 手續費 </param>
        public CollisionAndLastStepPadding(ICollection<ICollection<Symbol>> allMarketMix, double fee)
            => (AllMarketMix, this.fee) = (allMarketMix, Convert.ToDecimal(fee / 100));

        /// <summary>
        /// 計算所有投資組合的收益
        /// </summary>
        /// <param name="startAsset"> 開始與結束的貨幣 </param>
        /// <param name="orderBooks"> 訂單簿 </param>
        /// <param name="lastestPrices"> 所有市場的最新價格 </param>
        /// <returns></returns>
        public async Task<ICollection<(ICollection<Symbol> marketMix, float assets)>> CalculateAllIncomeAsync(string startAsset, IDictionary<string, OrderBook> orderBooks, IDictionary<string, LatestPrice> lastestPrices)
        {
            IEnumerable<Task<(ICollection<Symbol> marketMix, float asset)>> tasks = AllMarketMix.AsParallel().Select(item => Task.Run<(ICollection<Symbol> marketMix, float asset)>(() => (item, CalculateIncome(startAsset, item, orderBooks, lastestPrices))));
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 計算指定投資組合的收益
        /// </summary>
        /// <param name="startAsset"> 開始與結束的貨幣 </param>
        /// <param name="marketMix"> 指定的投資組合 </param>
        /// <param name="orderBooks"> 訂單簿 </param>
        /// <param name="lastestPrices"> 所有市場的最新價格 </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public float CalculateIncome(string startAsset, ICollection<Symbol> marketMix, IDictionary<string, OrderBook> orderBooks, IDictionary<string, LatestPrice> lastestPrices)
        {
            decimal initialAssets = 1000000;
            decimal assets = initialAssets;
            string coin = startAsset;
            foreach (var result in marketMix.Select((Value, Index) => new { Value, Index }))
            {
                if (coin != result.Value.BaseAsset && coin != result.Value.QuoteAsset)
                    throw new ArgumentException($"Coin: {coin} not in symbol: {result.Value.Name}, MarketMix: {string.Join(", ", marketMix.Select(item => item.Name))}");
                if (orderBooks[result.Value.Name].AskPrice is 0 || orderBooks[result.Value.Name].BidPrice is 0)
                    return float.MinValue;
                if (coin == result.Value.QuoteAsset)
                {
                    assets /= result.Index == marketMix.Count - 1 ? lastestPrices[result.Value.Name].Price : orderBooks[result.Value.Name].AskPrice;
                    coin = result.Value.BaseAsset;
                }
                else if (coin == result.Value.BaseAsset)
                {
                    assets *= result.Index == marketMix.Count - 1 ? lastestPrices[result.Value.Name].Price : orderBooks[result.Value.Name].BidPrice;
                    coin = result.Value.QuoteAsset;
                }
                assets -= assets * fee;
            }

            return Convert.ToSingle((assets - initialAssets) / initialAssets * 100);
        }
    }
}
