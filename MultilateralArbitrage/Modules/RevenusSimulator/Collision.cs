using MultilateralArbitrage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultilateralArbitrage.Modules.RevenusSimulator
{
    /// <summary>
    /// 模擬強撞掛單的收益模組
    /// </summary>
    internal class Collision
    {
        private readonly decimal fee;
        public ICollection<ICollection<Symbol>> AllMarketMix { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="allMarketMix"> 所有的投資組合 </param>
        /// <param name="fee"> 手續費 </param>
        public Collision(ICollection<ICollection<Symbol>> allMarketMix, double fee)
            => (AllMarketMix, this.fee) = (allMarketMix, Convert.ToDecimal(fee / 100));

        /// <summary>
        /// 計算所有投資組合的收益
        /// </summary>
        /// <param name="startAsset"> 開始與結束的貨幣 </param>
        /// <param name="orderBooks"> 訂單簿 </param>
        /// <returns></returns>
        public async Task<ICollection<(ICollection<Symbol> marketMix, float assets)>> CalculateAllIncomeAsync(string startAsset, IDictionary<string, OrderBook> orderBooks)
        {
            IEnumerable<Task<(ICollection<Symbol> marketMix, float asset)>> tasks = AllMarketMix.AsParallel().Select(item => Task.Run<(ICollection<Symbol> marketMix, float asset)>(() => (item, CalculateIncome(startAsset, item, orderBooks))));
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 計算指定投資組合的收益
        /// </summary>
        /// <param name="startAsset"> 開始與結束的貨幣 </param>
        /// <param name="marketMix"> 指定的投資組合 </param>
        /// <param name="orderBooks"> 訂單簿 </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public float CalculateIncome(string startAsset, ICollection<Symbol> marketMix, IDictionary<string, OrderBook> orderBooks)
        {
            decimal initialAssets = 1000000;
            decimal assets = initialAssets;
            string coin = startAsset;
            foreach (Symbol symbol in marketMix)
            {
                if (coin != symbol.BaseAsset && coin != symbol.QuoteAsset)
                    throw new ArgumentException($"Coin: {coin} not in symbol: {symbol.Name}, MarketMix: {string.Join(", ", marketMix.Select(item => item.Name))}");
                if (orderBooks[symbol.Name].AskPrice is 0 || orderBooks[symbol.Name].BidPrice is 0)
                    return float.MinValue;
                if (coin == symbol.QuoteAsset)
                {
                    assets /= orderBooks[symbol.Name].AskPrice;
                    coin = symbol.BaseAsset;
                }
                else if (coin == symbol.BaseAsset)
                {
                    assets *= orderBooks[symbol.Name].BidPrice;
                    coin = symbol.QuoteAsset;
                }
                assets -= assets * fee;
            }

            return Convert.ToSingle((assets - initialAssets) / initialAssets * 100);
        }
    }
}
