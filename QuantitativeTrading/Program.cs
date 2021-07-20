using System.IO;
using System.Threading.Tasks;
using QuantitativeTrading.Data.DataLoaders;
using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Environments;
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

        static async Task Main(string[] args)
        {
            ThreeMarketsDatasetModel dataset = await ThreeMarketsDataLoader.LoadCsvDataAsync(Path.Combine(datasetPath, "BTCUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHBTC-Spot.csv"));
            ThreeMarketsDataProvider dataProvider = new(dataset);
            EnvironmentParams environmentParams = new(20000, 10000, 0.1m, 3);
            SpotEnvironment env = new(dataProvider, environmentParams);
            AutoParamsCloseChangeRunner<AutoParamsCloseChange, AutoParamsCloseChangeRecordModel> runner = new(new(), env, new("AutoParamsCloseChangeRunner", savePath));
            await runner.RunAsync();
        }
    }
}
