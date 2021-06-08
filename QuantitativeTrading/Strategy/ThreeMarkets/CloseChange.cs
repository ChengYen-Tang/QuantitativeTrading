using QuantitativeTrading.Models;

namespace QuantitativeTrading.Strategy.ThreeMarkets
{
    public class CloseChange : Strategy, IStrategy
    {
        public CloseChange(int bufferSize)
            :base(bufferSize) { }

        public StrategyAction PolicyDecision(ThreeMarketsDataProviderModel model)
        {
            buffer.Enqueue(model);
            if (buffer.Count < bufferSize)
                return StrategyAction.WaitBuffer;

            if (Coin1ToCoinChange < 0 && Coin2ToCoinChange < 0)
                return StrategyAction.Coin;
            if (Coin1ToCoinChange > Coin2ToCoinChange)
                return StrategyAction.Coin1;
            else
                return StrategyAction.Coin2;
        }
    }
}
