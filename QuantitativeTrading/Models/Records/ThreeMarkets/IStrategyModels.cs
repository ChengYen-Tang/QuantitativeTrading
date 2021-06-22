namespace QuantitativeTrading.Models.Records.ThreeMarkets
{
    public interface ICloseChangeSumModels : IStrategyModels
    {
        public decimal Coin1ToCoinChangeSum { get; set; }
        public decimal Coin2ToCoinChangeSum { get; set; }
    }
}
