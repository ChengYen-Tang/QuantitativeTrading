using Magicodes.ExporterAndImporter.Csv;
using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Models.Records.ThreeMarkets;
using QuantitativeTrading.Strategies.ThreeMarkets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IEnvironmentModels = QuantitativeTrading.Models.Records.ThreeMarkets.IEnvironmentModels;

namespace QuantitativeTrading.Runners.ThreeMarkets
{
    /// <summary>
    /// 執行所有參數，尋找最佳參數
    /// </summary>
    public static class RunAllParams
    {
        private static volatile int counter = 0;

        /// <summary>
        /// 運行 CloseChangeSum 策略的參數
        /// </summary>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="environmentParams"> 回測環境的參數 </param>
        /// <param name="observationTimes"> 使用歷史多久的時間觀察 </param>
        /// <param name="tradingIntervals"> 交易頻率(多久交易一次) </param>
        /// <param name="savePath"> 存檔位置 </param>
        /// <returns></returns>
        public static async Task RunCloseChangeSumAllParams(ThreeMarketsDataProvider dataProvider, EnvironmentParams environmentParams, int[] observationTimes, int[] tradingIntervals, string savePath)
        {
            List<(int observationTime, int tradingInterval)> combinations = new();
            ConcurrentBag<ThreeMarketsCombinationModels> results = new();
            foreach (int observationTime in observationTimes)
                foreach (int tradingInterval in tradingIntervals)
                    combinations.Add((observationTime, tradingInterval));

            Parallel.ForEach(combinations, new ParallelOptions { MaxDegreeOfParallelism = 6 }, (combination) =>
            {
                CloseChangeSum strategy = new(combination.observationTime, combination.tradingInterval);
                string combinationName = $"{Utils.MinuteToHrOrDay(combination.observationTime)}-{Utils.MinuteToHrOrDay(combination.tradingInterval)}";
                ThreeMarketsCombinationModels result = RunParams<CloseChangeSumRecordModel>(dataProvider, strategy, environmentParams, combinationName, savePath).Result;
                results.Add(result);
                counter++;
                Console.WriteLine(counter);
            });

            var resultsArray = results.ToArray();
            await new CsvExporter().Export(Path.Combine(savePath, "CombinationResult.csv"), resultsArray);
        }

        /// <summary>
        /// 開始回測
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="strategy"> 策略 </param>
        /// <param name="environmentParams"> 回測環境的參數 </param>
        /// <param name="combination"> 回測紀錄名稱 </param>
        /// <param name="savePath"> 存檔位置 </param>
        /// <returns></returns>
        private static async Task<ThreeMarketsCombinationModels> RunParams<V>(ThreeMarketsDataProvider dataProvider, Strategy strategy, EnvironmentParams environmentParams, string combination, string savePath)
            where V : class, IEnvironmentModels, IStrategyModels, new()
        {
            ThreeMarketsDataProvider newDataProvider = dataProvider.Clone();

            SpotEnvironment env = new(newDataProvider, environmentParams);
            Runner<Strategy, V> runner = new(strategy, env, new(combination, savePath));
            await runner.RunAsync();
            return new() { Combination = combination, Assets = env.Assets, Balance = env.Balance, CoinBalance1 = env.CoinBalance1, CoinBalance2 = env.CoinBalance2, EndDate = env.CurrentKline.Coin22Coin1Kline.Date };
        }
    }
}
