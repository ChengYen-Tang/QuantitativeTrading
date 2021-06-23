﻿using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Models.Records.ThreeMarkets;

namespace QuantitativeTrading.Strategies.ThreeMarkets
{
    public class CloseChangeSum : Strategy
    {
        private decimal Coin1ToCoinChange => buffer.Count > 1 ? (buffer.Last.Coin12CoinKline.Close - buffer.First.Coin12CoinKline.Close) / buffer.First.Coin12CoinKline.Close : 0;
        private decimal Coin2ToCoinChange => buffer.Count > 1 ? (buffer.Last.Coin22CoinKline.Close - buffer.First.Coin22CoinKline.Close) / buffer.First.Coin22CoinKline.Close : 0;

        public CloseChangeSum(int bufferSize, int tradingInterval)
            : base(bufferSize, tradingInterval) { }

        public override StrategyAction PolicyDecision(ThreeMarketsDataProviderModel model)
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

        public override void Recording(IStrategyModels record)
        {
            ICloseChangeSumModels closeChangeSumRecord = record as ICloseChangeSumModels;
            closeChangeSumRecord.Coin1ToCoinChangeSum = Coin1ToCoinChange;
            closeChangeSumRecord.Coin2ToCoinChangeSum = Coin2ToCoinChange;
        }
    }
}
