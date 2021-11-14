using MultilateralArbitrage.Models;

namespace MultilateralArbitrage.Modules
{
    internal static class Extend
    {
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
