using System.Linq;
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
            decimal coin1ToCoinChange = buffer.Select(item => item.Coin12CoinKline.Change).Sum();
            decimal coin2ToCoinChange = buffer.Select(item => item.Coin22CoinKline.Change).Sum();

            if (buffer.Count < bufferSize)
                return StrategyAction.WaitBuffer;

            if (coin1ToCoinChange < 0 && coin2ToCoinChange < 0)
                return StrategyAction.Coin;
            if (coin1ToCoinChange > coin2ToCoinChange)
                return StrategyAction.Coin1;
            else
                return StrategyAction.Coin2;
        }
    }
}
