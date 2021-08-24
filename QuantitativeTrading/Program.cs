using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Services.ThreeMarkets;

namespace QuantitativeTrading
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //using BinanceSpot binanceSpot= new("USDT", "BTC", "ETH");
            //binanceSpot.Run();

            //while(true)
            //{
            //    var test = await binanceSpot.GetKlineAsync();
            //    Console.WriteLine(test.Coin12CoinKline.Date);
            //    Console.WriteLine(test.Coin22CoinKline.Date);
            //    Console.WriteLine(test.Coin22Coin1Kline.Date);
            //    Console.WriteLine("--------------------------------");
            //}

            //Console.ReadKey();
            //await binanceSpot.CloseAsync();
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
