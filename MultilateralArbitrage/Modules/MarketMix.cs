using MultilateralArbitrage.Models;

namespace MultilateralArbitrage.Modules
{
    internal class MarketMix
    {
        private readonly IDictionary<string, ICollection<Symbol>> symbols;
        private readonly int depth;

        public MarketMix(IDictionary<string, ICollection<Symbol>> symbols, int depth = 0)
            => (this.symbols, this.depth) = (symbols, depth);

        public ICollection<ICollection<Symbol>> GetAllMarketMix(string startAsset)
        {
            List<ICollection<Symbol>> allMarketMix = new();
            Ecplore(startAsset, startAsset, 0, new(), allMarketMix);
            return allMarketMix;
        }

        private void Ecplore(string startAsset, string ecploreAsset, int nowDepth, List<Symbol> marketMix, List<ICollection<Symbol>> allMarketMix)
        {
            if (depth is not 0 && nowDepth == depth)
                return;

            foreach (Symbol binanceSymbol in symbols[ecploreAsset])
            {
                if (marketMix.Contains(binanceSymbol))
                    continue;

                List<Symbol> buffer = new(marketMix);
                buffer.Add(binanceSymbol);
                string asset = ecploreAsset == binanceSymbol.BaseAsset ? binanceSymbol.QuoteAsset : binanceSymbol.BaseAsset;
                if (asset == startAsset)
                {
                    allMarketMix.Add(buffer);
                    continue;
                }

                Ecplore(startAsset, asset, nowDepth + 1, buffer, allMarketMix);
            }
        }
    }
}
