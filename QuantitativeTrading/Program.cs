using System;
using System.IO;
using System.Threading.Tasks;
using QuantitativeTrading.DataProvider;
using QuantitativeTrading.Environment;
using QuantitativeTrading.Models;
using QuantitativeTrading.Runner;
using QuantitativeTrading.Strategy.ThreeMarkets;

namespace QuantitativeTrading
{
    class Program
    {
        private const string datasetPath = "/Users/kenneth/OneDrive - 臺北科技大學 軟體工程實驗室/量化交易/General/原始資料集";

        static async Task Main(string[] args)
        {
            ThreeMarketsModel model = await ThreeMarketsModel.CreateModel(Path.Combine(datasetPath, "BTCUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHBTC-Spot.csv"));
            ThreeMarketsDataProvider dataProvider = new(model);
            ThreeMarketsEnvironment env = new(dataProvider, 20000, 10000, 0.1m, 3);
            ThreeMarketsRunner<CloseChange> runner = new(new(28800, 1440), env, new("5min", AppDomain.CurrentDomain.BaseDirectory));
            await runner.RunAsync();
            Console.WriteLine($"Assets: {env.Assets}");
            Console.WriteLine($"Balance: {env.Balance}");
            Console.WriteLine($"CoinBalance1: {env.CoinBalance1}");
            Console.WriteLine($"CoinBalance2: {env.CoinBalance2}");
            Console.WriteLine($"Date: {dataProvider.Current.Coin12CoinKline.Date}");
        }
    }
}
