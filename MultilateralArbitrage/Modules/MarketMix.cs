using MultilateralArbitrage.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultilateralArbitrage.Modules
{
    /// <summary>
    /// 產生投資組合的模組
    /// </summary>
    internal class MarketMix
    {
        private readonly ICollection<string> excludeCoin;
        private readonly IDictionary<string, ICollection<Symbol>> symbols;
        private readonly int depth;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="symbols"> 所有市場 </param>
        /// <param name="depth"> 瓊舉深度 </param>
        public MarketMix(IDictionary<string, ICollection<Symbol>> symbols, int depth = 0)
            : this(symbols, Array.Empty<string>(), depth) {}

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="symbols"> 所有市場 </param>
        /// <param name="excludeCoin"> 想要排除的市場 </param>
        /// <param name="depth"> 瓊舉深度 </param>
        public MarketMix(IDictionary<string, ICollection<Symbol>> symbols, ICollection<string> excludeCoin, int depth = 0)
            => (this.symbols, this.excludeCoin, this.depth) = (symbols, excludeCoin, depth);

        /// <summary>
        /// 產生投資組合
        /// </summary>
        /// <param name="startAsset"> 開始與結束的貨幣 </param>
        /// <returns></returns>
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
                if (excludeCoin.Any() && excludeCoin.Contains(asset))
                    continue;
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
