using MultilateralArbitrage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultilateralArbitrage.Modules.RevenusSimulator
{
    internal class CollisionAndLastStepPadding
    {
        private readonly decimal fee;
        public ICollection<ICollection<Symbol>> AllMarketMix { get; set; }
        public CollisionAndLastStepPadding(ICollection<ICollection<Symbol>> allMarketMix, double fee)
            => (AllMarketMix, this.fee) = (allMarketMix, Convert.ToDecimal(fee / 100));

        public async Task<ICollection<(ICollection<Symbol> marketMix, float assets)>> CalculateAllIncomeAsync(string startAsset, IDictionary<string, OrderBook> orderBooks, IDictionary<string, LatestPrice> lastestPrice)
        {
            IEnumerable<Task<(ICollection<Symbol> marketMix, float asset)>> tasks = AllMarketMix.AsParallel().Select(item => Task.Run<(ICollection<Symbol> marketMix, float asset)>(() => (item, CalculateIncome(startAsset, item, orderBooks, lastestPrice))));
            return await Task.WhenAll(tasks);
        }

        public float CalculateIncome(string startAsset, ICollection<Symbol> marketMix, IDictionary<string, OrderBook> orderBooks, IDictionary<string, LatestPrice> lastestPrice)
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
                    assets /= result.Index == marketMix.Count - 1 ? lastestPrice[result.Value.Name].Price : orderBooks[result.Value.Name].AskPrice;
                    coin = result.Value.BaseAsset;
                }
                else if (coin == result.Value.BaseAsset)
                {
                    assets *= result.Index == marketMix.Count - 1 ? lastestPrice[result.Value.Name].Price : orderBooks[result.Value.Name].BidPrice;
                    coin = result.Value.QuoteAsset;
                }
                assets -= assets * fee;
            }

            return Convert.ToSingle((assets - initialAssets) / initialAssets * 100);
        }
    }
}
