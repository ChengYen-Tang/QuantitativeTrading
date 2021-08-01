using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuantitativeTrading.Environments.ThreeMarkets;

namespace QuantitativeTrading
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IEnumerable<string> symbols = new[] { "BTCUSDT", "ETHUSDT", "ETHBTC" };
            using BinanceSpot binanceSpot= new(symbols);
            binanceSpot.Run();

            while(true)
            {
                var test = await binanceSpot.GetKlineAsync();
                Console.WriteLine(test.Coin12CoinKline.Date);
                Console.WriteLine(test.Coin22CoinKline.Date);
                Console.WriteLine(test.Coin22Coin1Kline.Date);
                Console.WriteLine("--------------------------------");
            }

            //Console.ReadKey();
            //await binanceSpot.CloseAsync();
        }
    }
}
