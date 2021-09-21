using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuantitativeTrading.Data.DataLoaders;
using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Models;
using QuantitativeTrading.Runners.ThreeMarkets;
using QuantitativeTrading.Services.ThreeMarkets;

namespace QuantitativeTrading
{
    class Program
    {
        private const string datasetPath = @"C:\Users\Kenneth\OneDrive - 臺北科技大學 軟體工程實驗室\量化交易\General\原始資料集";
        private const string savePath = @"E:\回測結果";

        static async Task Main(string[] args)
        {
            int[] observationTimes = new[] { 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440, 4320, 10080, 20160, 30240, 40320 };
            int[] tradingIntervals = new[] { 1, 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440 };
            decimal[] sellConditions = new[] { -1M, -2M, -3M, -4M, -5M, -6M, -7M, -8M, -9M, -10M, -11M, -12M, -13M, -14M, -15M };
            ThreeMarketsDatasetModel dataset = await ThreeMarketsDataLoader.LoadCsvDataAsync(Path.Combine(datasetPath, "BTCUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHBTC-Spot.csv"));
            ThreeMarketsDataProvider dataProvider = new(dataset);
            EnvironmentParams environmentParams = new(20000, 10000, 0.1m, 3);

            await RunAllParams.RunAutoSellCloseChangeAllParams(dataProvider, environmentParams, observationTimes, tradingIntervals, sellConditions, savePath);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .ConfigureHostConfiguration(configHost =>
                    {
                        configHost.AddEnvironmentVariables();
                        configHost.AddCommandLine(args);
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<BinanceService>();
                    });
    }
}
