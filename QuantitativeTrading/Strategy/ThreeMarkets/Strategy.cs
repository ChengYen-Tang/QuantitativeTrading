using QuantitativeTrading.Models;
using System;
using System.Linq;

namespace QuantitativeTrading.Strategy.ThreeMarkets
{
    public abstract class Strategy
    {
        protected readonly int bufferSize;
        protected FixedSizeQueue<ThreeMarketsDataProviderModel> buffer;

        public Strategy(int bufferSize)
            => (this.bufferSize, buffer) = (bufferSize, new(bufferSize));

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
