using MultilateralArbitrage.Models;
using System.Collections.Generic;

namespace MultilateralArbitrage.Modules
{
    internal static class Extend
    {
        /// <summary>
        /// 將市場使用字典分類
        /// 
        /// 例: ETHBTC, BTCUSDT
        /// key = USDT: values = BTCUSDT
        /// key = BTC: values = ETHBTC, BTCUSDT
        /// key = ETH: values = ETHBTC
        /// </summary>
        /// <param name="symbols"> 要分類的市場 </param>
        /// <returns></returns>
        public static IDictionary<string, ICollection<Symbol>> ToClassificationSymbols(this ICollection<Symbol> symbols)
        {
            Dictionary<string, ICollection<Symbol>> classificationSymbol = new();
            foreach (Symbol symbol in symbols)
            {
                if (!classificationSymbol.ContainsKey(symbol.BaseAsset))
                    classificationSymbol.Add(symbol.BaseAsset, new List<Symbol>());
                if (!classificationSymbol.ContainsKey(symbol.QuoteAsset))
                    classificationSymbol.Add(symbol.QuoteAsset, new List<Symbol>());
                classificationSymbol[symbol.BaseAsset].Add(symbol);
                classificationSymbol[symbol.QuoteAsset].Add(symbol);
            }
            return classificationSymbol;
        }
    }
}
