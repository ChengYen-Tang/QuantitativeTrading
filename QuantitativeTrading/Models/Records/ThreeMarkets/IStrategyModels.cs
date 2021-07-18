namespace QuantitativeTrading.Models.Records.ThreeMarkets
{
    public interface ICloseChangeModels : IStrategyModels
    {
        public decimal Coin1ToCoinChangeSum { get; set; }
        public decimal Coin2ToCoinChangeSum { get; set; }
    }

    public interface IAutoParamsCloseChange : ICloseChangeModels
    {
        public int ObservationTime { get; set; }
        public int TradingInterval { get; set; }
    }
}
