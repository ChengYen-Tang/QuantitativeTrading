using System.Linq;
using QuantitativeTrading.Models;

namespace QuantitativeTrading.Strategy.ThreeMarkets
{
    public class CloseChange : Strategy, IStrategy
    {
        public CloseChange(int bufferSize, int tradingInterval)
            :base(bufferSize, tradingInterval) { }

        public StrategyAction PolicyDecision(ThreeMarketsDataProviderModel model)
        {
            buffer.Enqueue(model);

            if (buffer.Count < bufferSize || !CanTrading())
                return StrategyAction.WaitBuffer;

            step = 0;
            if (Coin1ToCoinChange < 0 && Coin2ToCoinChange < 0)
                return StrategyAction.Coin;
            if (Coin1ToCoinChange > Coin2ToCoinChange)
                return StrategyAction.Coin1;
            else
                return StrategyAction.Coin2;
        }
    }
}
