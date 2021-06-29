﻿using System.IO;
using System.Threading.Tasks;
using QuantitativeTrading.Data.DataLoaders;
using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Models;
using QuantitativeTrading.Runners.ThreeMarkets;

namespace QuantitativeTrading
{
    class Program
    {
        private const string datasetPath = @"C:\Users\Kenneth\OneDrive - 臺北科技大學 軟體工程實驗室\量化交易\General\原始資料集";
        private const string savePath = @"C:\Users\Kenneth\OneDrive - 臺北科技大學 軟體工程實驗室\回測結果";

        static async Task Main(string[] args)
        {
            
            int[] observationTimes = new int[] { 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440, 4320, 10080, 20160, 30240, 40320 };
            int[] tradingIntervals = new int[] { 1, 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440 };
            ThreeMarketsDatasetModel dataset = await ThreeMarketsDataLoader.LoadCsvDataAsync(Path.Combine(datasetPath, "BTCUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHBTC-Spot.csv"));
            ThreeMarketsDataProvider dataProvider = new(dataset);
            EnvironmentParams environmentParams = new(20000, 10000, 0.1m, 3);

            await RunAllParams.RunCloseChangeAllParams(dataProvider, environmentParams, observationTimes, tradingIntervals, savePath);
        }
    }
}
