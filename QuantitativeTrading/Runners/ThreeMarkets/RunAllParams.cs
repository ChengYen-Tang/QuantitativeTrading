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
    public static class RunAllParams
    {
        private static volatile int counter = 0;

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
