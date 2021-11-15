using MultilateralArbitrage.Models;

namespace MultilateralArbitrage.Modules
{
    internal class RevenusSimulator
    {
        private readonly decimal fee;
        public ICollection<ICollection<Symbol>> AllMarketMix { get; set; }
        public RevenusSimulator(ICollection<ICollection<Symbol>> allMarketMix, int fee)
            => (AllMarketMix, this.fee) = (allMarketMix, fee / 100);

        public async Task<ICollection<(ICollection<Symbol> marketMix, decimal assets)>> CalculateAllIncomeAsync(string startAsset, IDictionary<string, OrderBook> orderBooks)
        {
            IEnumerable<Task<(ICollection<Symbol> marketMix, decimal asset)>> tasks = AllMarketMix.AsParallel().Select(item => Task.Run<(ICollection<Symbol> marketMix, decimal asset)>(() => (item, CalculateIncome(startAsset, item, orderBooks))));
            return await Task.WhenAll(tasks);
        }

        public decimal CalculateIncome(string startAsset, ICollection<Symbol> marketMix, IDictionary<string, OrderBook> orderBooks)
        {
            decimal initialAssets = 1000000;
            decimal assets = initialAssets;
            string coin = startAsset;
            foreach (Symbol symbol in marketMix)
            {
                if (coin != symbol.BaseAsset && coin != symbol.QuoteAsset)
                    throw new ArgumentException($"Coin: {coin} not in symbol: {symbol.Name}, MarketMix: {string.Join(", ", marketMix.Select(item => item.Name))}");
                if (orderBooks[symbol.Name].AskPrice is 0 || orderBooks[symbol.Name].BidPrice is 0)
                    return decimal.MinValue;
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

            return (assets - initialAssets) / initialAssets * 100;
        }
    }
}
