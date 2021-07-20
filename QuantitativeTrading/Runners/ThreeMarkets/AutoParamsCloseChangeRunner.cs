﻿using QuantitativeTrading.Environments;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Strategies;
using QuantitativeTrading.Strategies.ThreeMarkets;
using System.Threading.Tasks;
using IEnvironmentModels = QuantitativeTrading.Models.Records.ThreeMarkets.IEnvironmentModels;
using Strategy = QuantitativeTrading.Strategies.ThreeMarkets.Strategy;

namespace QuantitativeTrading.Runners.ThreeMarkets
{
    public class AutoParamsCloseChangeRunner<T, U> : Runner<T, U>
        where T : Strategy, IAutoParams
        where U : class, IEnvironmentModels, IStrategyModels, new()
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="strategy"> 策略 </param>
        /// <param name="environment"> 回測環境 </param>
        /// <param name="recorder"> 交易紀錄器 </param>
        public AutoParamsCloseChangeRunner(T strategy, SpotEnvironment environment, Recorder<U> recorder)
            : base(strategy, environment, recorder) { }

        /// <summary>
        /// 開始回測，直到資料集結束或設定環境的低於最低餘額
        /// </summary>
        /// <returns></returns>
        public override async Task RunAsync()
        {
            EnvironmentParams environmentParams = new(20000, 10000, 0.1m, 3);

            while (!environment.IsGameOver)
            {
                if (environment.CurrentKline.Coin12CoinKline.Date.Day == 1 && environment.CurrentKline.Coin12CoinKline.Date.Hour == 0 && environment.CurrentKline.Coin12CoinKline.Date.Minute == 0)
                {
                    (int observationTime, int tradingInterval) = RunAllParams.RunFindAutoParamsCloseChangeBestParams(environment.CloneCurrentDataProvider(), environmentParams);
                    strategy.UpdateParams(observationTime, tradingInterval);
                }

                ThreeMarketsDataProviderModel data = environment.CurrentKline;
                StrategyAction action = strategy.PolicyDecision(data);
                Trading(action);
                if (recorder is not null)
                {
                    U record = new();
                    environment.Recording(record);
                    strategy.Recording(record);
                    recorder.Insert(record);
                }
                environment.MoveNextTime(out _);
            }

            if (recorder is not null)
                await recorder.SaveAsync();
        }
    }
}