using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Magicodes.ExporterAndImporter.Csv;
using QuantitativeTrading.Data.DataLoaders;
using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records.ThreeMarkets;
using QuantitativeTrading.Runners.ThreeMarkets;
using QuantitativeTrading.Strategies.ThreeMarkets;

namespace QuantitativeTrading
{
    class Program
    {
        private const string datasetPath = @"C:\Users\Kenneth\OneDrive - 臺北科技大學 軟體工程實驗室\量化交易\General\原始資料集";
        private const string savePath = @"C:\Users\Kenneth\OneDrive - 臺北科技大學 軟體工程實驗室\回測結果";
        private static volatile int counter = 0;

        static async Task Main(string[] args)
        {
            
            int[] observationTimes = new int[] { 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440, 4320, 10080, 20160, 30240, 40320 };
            int[] tradingIntervals = new int[] { 1, 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440 };

            List<(int observationTime, int tradingInterval)> combinations = new();
            ConcurrentBag<CloseChangeSumCombinationModels> results = new();
            foreach (int observationTime in observationTimes)
                foreach (int tradingInterval in tradingIntervals)
                    combinations.Add((observationTime, tradingInterval));

            ThreeMarketsDatasetModel dataset = await ThreeMarketsDataLoader.LoadCsvDataAsync(Path.Combine(datasetPath, "BTCUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHBTC-Spot.csv"));
            ThreeMarketsDataProvider dataProvider = new(dataset);

            Parallel.ForEach(combinations, new ParallelOptions { MaxDegreeOfParallelism = 6 }, (combination) =>
            {
                CloseChangeSumCombinationModels result = CloseChangeSum(dataProvider, combination.observationTime, combination.tradingInterval).Result;
                results.Add(result);
                counter++;
                Console.WriteLine(counter);
            });

            var resultsArray = results.ToArray();
            await new CsvExporter().Export(Path.Combine(savePath, "CombinationResult.csv"), resultsArray);
        }

        static async Task<CloseChangeSumCombinationModels> CloseChangeSum(ThreeMarketsDataProvider dataProvider, int observationTime , int tradingInterval)
        {
            ThreeMarketsDataProvider newDataProvider = dataProvider.Clone();
            string combination = $"{MinuteToString(observationTime)}-{MinuteToString(tradingInterval)}";
            SpotEnvironment env = new(newDataProvider, 20000, 10000, 0.1m, 3);
            Runner<CloseChangeSum, CloseChangeSumRecordModel> runner = new(new(observationTime, tradingInterval), env, new(combination, savePath));
            await runner.RunAsync();
            return new() { Combination = combination, Assets = env.Assets, Balance = env.Balance, CoinBalance1 = env.CoinBalance1, CoinBalance2 = env.CoinBalance2, EndDate = env.CurrentKline.Coin22Coin1Kline.Date };
        }

        static string MinuteToString(int minute)
        {
            if (minute >= 60)
                return $"{minute / 60}Hr";
            if (minute >= 1440)
                return $"{minute / 1440}Hr";
            return $"{minute}Min";
        }
    }
}
