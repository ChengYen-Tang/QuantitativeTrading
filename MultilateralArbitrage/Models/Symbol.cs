namespace MultilateralArbitrage.Models
{
    internal class Symbol
    {
        public string Name { get; set; } = default!;
        public string BaseAsset { get; set; } = default!;
        public string QuoteAsset { get; set; } = default!;
    }
}