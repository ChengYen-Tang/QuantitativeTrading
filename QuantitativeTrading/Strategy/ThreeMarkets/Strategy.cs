using QuantitativeTrading.Models;
using System;
using System.Linq;

namespace QuantitativeTrading.Strategy.ThreeMarkets
{
    public abstract class Strategy
    {
        protected readonly int bufferSize;
        protected decimal Coin1ToCoinChange { get { return buffer.Select(item => item.Coin12CoinKline.Change).Sum(); } }
        protected decimal Coin2ToCoinChange { get { return buffer.Select(item => item.Coin22CoinKline.Change).Sum(); } }
        protected FixedSizeQueue<ThreeMarketsDataProviderModel> buffer;

        public Strategy(int bufferSize)
            => (this.bufferSize, buffer) = (bufferSize, new(bufferSize));

        public BestPath BestCoin1ToCoin2Path(StrategyAction strategyAction)
        {
            decimal change1 = Coin2ToCoinChange - Coin1ToCoinChange;
            decimal change2 = buffer.Select(item => item.Coin22Coin1Kline.Change).Sum();

            if (strategyAction == StrategyAction.Coin1)
                return change1 > change2 ? BestPath.Path1 : BestPath.Path2;


            if (strategyAction == StrategyAction.Coin2)
                return change2 > change1 ? BestPath.Path1 : BestPath.Path2;

            throw new Exception("輸入只允許 Coin1 or Coin2");
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
        Path1 = 0,
        Path2
    }
}
