﻿using QuantitativeTrading.Models;
using System.Collections.Generic;
using System.Linq;

namespace QuantitativeTrading.Component.DataProvider
{
    public class ThreeMarketsDataProvider : KlineDataProvider<ThreeMarketsDataProviderModel>
    {
        public ThreeMarketsDataProvider(ThreeMarketsModel model)
            => models = Join(model);

        private static List<ThreeMarketsDataProviderModel> Join(ThreeMarketsModel model)
            => model.Coin12CoinKlines.AsParallel()
            .Join(model.Coin22CoinKlines.AsParallel(), B2AKline => B2AKline.Date, C2AKline => C2AKline.Date, (B2AKline, C2AKline) => new { B2AKline, C2AKline })
            .Join(model.Coin22Coin1Klines.AsParallel(), item => item.B2AKline.Date, C2BKline => C2BKline.Date, (item, C2BKline) => new ThreeMarketsDataProviderModel { Coin12CoinKline = item.B2AKline, Coin22CoinKline = item.C2AKline, Coin22Coin1Kline = C2BKline })
            .OrderBy(item => item.Coin12CoinKline.Date).ToList();
    }
}
