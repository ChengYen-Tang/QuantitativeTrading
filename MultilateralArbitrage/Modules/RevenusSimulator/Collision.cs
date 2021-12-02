using MultilateralArbitrage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultilateralArbitrage.Modules.RevenusSimulator
{
    internal class Collision
    {
        private readonly decimal fee;
        public ICollection<ICollection<Symbol>> AllMarketMix { get; set; }
        public Collision(ICollection<ICollection<Symbol>> allMarketMix, double fee)
            => (AllMarketMix, this.fee) = (allMarketMix, Convert.ToDecimal(fee / 100));

        public async Task<ICollection<(ICollection<Symbol> marketMix, float assets)>> CalculateAllIncomeAsync(string startAsset, IDictionary<string, OrderBook> orderBooks)
        {
            IEnumerable<Task<(ICollection<Symbol> marketMix, float asset)>> tasks = AllMarketMix.AsParallel().Select(item => Task.Run<(ICollection<Symbol> marketMix, float asset)>(() => (item, CalculateIncome(startAsset, item, orderBooks))));
            return await Task.WhenAll(tasks);
        }

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
