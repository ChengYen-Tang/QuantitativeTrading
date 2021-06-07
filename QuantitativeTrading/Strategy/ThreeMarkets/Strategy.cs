using QuantitativeTrading.Environment;
using QuantitativeTrading.Models;

namespace QuantitativeTrading.Strategy.ThreeMarkets
{
    public abstract class Strategy
    {
        protected readonly int bufferSize;
        protected FixedSizeQueue<ThreeMarketsDataProviderModel> buffer;

        public Strategy(int bufferSize)
            => (this.bufferSize, buffer) = (bufferSize, new(bufferSize));
    }

    public enum StrategyAction
    {
        WaitBuffer = 0,
        Coin,
        Coin1,
        Coin2
    }
}
