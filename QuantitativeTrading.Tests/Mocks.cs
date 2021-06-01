using QuantitativeTrading.DataProvider;
using QuantitativeTrading.Models;
using System;

namespace QuantitativeTrading.Tests
{
    public static class Mocks
    {
        public static ThreeMarketsDataProvider ThreeMarketsDataProviderMock()
            => new(CreateThreeMarketsModelMock());

        public static ThreeMarketsModel CreateThreeMarketsModelMock()
        {
            DateTime now = DateTime.Now;
            KlineModel[] Coin12CoinKlines = new KlineModel[] { 
                new() { Close = 10, Date = now }, 
                new() { Close = 11, Date = now.AddMinutes(1) }, 
                new() { Close = 9, Date = now.AddMinutes(2) }, 
                new() { Close = 10, Date = now.AddMinutes(3) }, 
                new() { Close = 12, Date = now.AddMinutes(4) } };
            KlineModel[] Coin22CoinKlines = new KlineModel[] {
                new() { Close = 0.3M, Date = now },
                new() { Close = 0.4M, Date = now.AddMinutes(1) },
                new() { Close = 0.3M, Date = now.AddMinutes(2) },
                new() { Close = 0.2M, Date = now.AddMinutes(3) },
                new() { Close = 0.3M, Date = now.AddMinutes(4) } };
            KlineModel[] Coin22Coin1Klines = new KlineModel[] {
                new() { Close = (0.3M / 10) + 0.001M, Date = now },
                new() { Close = (0.4M / 11) - 0.002M, Date = now.AddMinutes(1) },
                new() { Close = (0.3M / 9) + 0.002M, Date = now.AddMinutes(2) },
                new() { Close = (0.2M / 10) - 0.001M, Date = now.AddMinutes(3) },
                new() { Close = (0.3M / 12) - 0.001M, Date = now.AddMinutes(4) } };

            return new(Coin12CoinKlines, Coin22CoinKlines, Coin22Coin1Klines);
        }
    }

}
