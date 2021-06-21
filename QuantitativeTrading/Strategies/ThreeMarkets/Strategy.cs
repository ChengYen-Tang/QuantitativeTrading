using QuantitativeTrading.Models;
using System;
using System.Linq;

namespace QuantitativeTrading.Strategies.ThreeMarkets
{
    public abstract class Strategy
    {
        protected readonly int bufferSize;
        protected int step = 0;
        protected int tradingInterval;
        protected FixedSizeQueue<ThreeMarketsDataProviderModel> buffer;

        public decimal Coin1ToCoinChange { get { return buffer.Select(item => item.Coin12CoinKline.Change).Sum(); } }
        public decimal Coin2ToCoinChange { get { return buffer.Select(item => item.Coin22CoinKline.Change).Sum(); } }

        public Strategy(int bufferSize, int tradingInterval)
            => (this.bufferSize, this.tradingInterval, buffer) = (bufferSize, tradingInterval, new(bufferSize));

        public abstract StrategyAction PolicyDecision(ThreeMarketsDataProviderModel model);

        public BestPath BestCoin1ToCoin2Path(StrategyAction strategyAction)
        {
            if (strategyAction == StrategyAction.Coin1)
            {
                decimal temp = 1 * buffer.Last().Coin22CoinKline.Close;
                return temp / buffer.Last().Coin12CoinKline.Close > 1 * buffer.Last().Coin22Coin1Kline.Close ? BestPath.Path1 : BestPath.Path2;
            }

            if (strategyAction == StrategyAction.Coin2)
            {
                decimal temp = 1 * buffer.Last().Coin12CoinKline.Close;
                return temp / buffer.Last().Coin22CoinKline.Close > 1 / buffer.Last().Coin22Coin1Kline.Close ? BestPath.Path1 : BestPath.Path2;
            }

            throw new Exception("輸入只允許 Coin1 or Coin2");
        }

        protected bool CanTrading()
        {
            if (step == tradingInterval)
            {
                step = 0;
                return true;
            }

            step++;
            return false;
        }
    }

    public enum StrategyAction
    {
        WaitBuffer = 0,
        Coin,
        Coin1,
        Coin2
    }

    public enum BestPath
    {
        // 3 point
        Path1 = 0,
        // 2 point
        Path2
    }
}
