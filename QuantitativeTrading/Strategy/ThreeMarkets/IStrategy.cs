using QuantitativeTrading.Models;

namespace QuantitativeTrading.Strategy.ThreeMarkets
{
    public interface IStrategy
    {
        StrategyAction PolicyDecision(ThreeMarketsDataProviderModel model);
    }
}
