﻿using QuantitativeTrading.Models;
using System;
using System.Runtime.CompilerServices;

namespace QuantitativeTrading.Strategies.ThreeMarkets
{
    /// <summary>
    /// 三市場策略
    /// </summary>
    public abstract class Strategy : Strategies.Strategy
    {
        protected readonly int bufferSize;
        protected int step = 0;
        protected int tradingInterval;
        protected FixedSizeQueue<ThreeMarketsDataProviderModel> buffer;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="bufferSize"> 需要觀察的天數 </param>
        /// <param name="tradingInterval"> 每次交易的間隔 </param>
        public Strategy(int bufferSize, int tradingInterval)
            => (this.bufferSize, this.tradingInterval, buffer) = (bufferSize, tradingInterval, new(bufferSize));

        /// <summary>
        /// 運行策略
        /// </summary>
        /// <param name="model"> 當下的市場資訊 </param>
        /// <returns></returns>
        public abstract StrategyAction PolicyDecision(ThreeMarketsDataProviderModel model);

        /// <summary>
        /// 從 Coin1 交易到 Coin2 最佳的路徑
        /// 
        /// 例: BTC -> ETH
        /// Path1: sell btc -> buy eth (3 point: btc -> usdt -> eth)
        /// Path2: buy eth (2 point: btc -> eth)
        /// </summary>
        /// <param name="strategyAction"></param>
        /// <returns></returns>
        public BestPath BestCoin1ToCoin2Path(StrategyAction strategyAction)
        {
            if (strategyAction == StrategyAction.Coin1)
            {
                decimal temp = 1 * buffer.Last.Coin22CoinKline.Close;
                return temp / buffer.Last.Coin12CoinKline.Close > 1 * buffer.Last.Coin22Coin1Kline.Close ? BestPath.Path1 : BestPath.Path2;
            }

            if (strategyAction == StrategyAction.Coin2)
            {
                decimal temp = 1 * buffer.Last.Coin12CoinKline.Close;
                return temp / buffer.Last.Coin22CoinKline.Close > 1 / buffer.Last.Coin22Coin1Kline.Close ? BestPath.Path1 : BestPath.Path2;
            }

            throw new Exception("輸入只允許 Coin1 or Coin2");
        }

        /// <summary>
        /// 是否到達交易間格次數
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
