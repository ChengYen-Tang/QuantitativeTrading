using QuantitativeTrading.Models.Records;

namespace QuantitativeTrading.Environments.ThreeMarkets
{
    public interface IThreeMarketEnvironment
    {
        decimal Assets { get; }
        decimal Balance { get; }
        decimal CoinBalance1 { get; }
        decimal CoinBalance2 { get; }

        void Recording(IEnvironmentModels record);
        void Trading(TradingAction action, TradingMarket market);
    }
}