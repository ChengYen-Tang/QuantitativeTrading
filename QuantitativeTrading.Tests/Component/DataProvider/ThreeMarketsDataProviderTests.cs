using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantitativeTrading.Component.DataProvider;
using QuantitativeTrading.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantitativeTrading.Tests.Component.DataProvider
{
    [TestClass]
    public class ThreeMarketsDataProviderTests
    {
        private readonly ThreeMarketsDataProvider provider;

        public ThreeMarketsDataProviderTests()
            => provider = new(ThreeMarketsModel.CreateModel(Utils.btc_usdtPath, Utils.eth_usdtPath, Utils.eth_btcPath).Result);

        [TestInitialize]
        public void Init()
            => provider.Reset();

        [TestMethod]
        public void TestLength()
            => Assert.AreEqual(309557, provider.Length);

        [TestMethod]
        public void TestOrder()
        {
            DateTime time = provider.Current.Coin12CoinKline.Date;
            while (provider.MoveNext(out ThreeMarketsDataProviderModel model))
            {
                Assert.IsTrue(model.Coin12CoinKline.Date > time);
                time = model.Coin12CoinKline.Date;
            }
        }

        [TestMethod]
        public void TestMarketDataLink()
        {
            foreach (ThreeMarketsDataProviderModel model in provider)
            {
                Assert.AreEqual(model.Coin12CoinKline.Date, model.Coin22CoinKline.Date);
                Assert.AreEqual(model.Coin22CoinKline.Date, model.Coin22Coin1Kline.Date);
            }
        }

        [TestMethod]
        public void TestIndex()
        {
            ThreeMarketsDataProviderModel currentModel = provider[1];
            provider.MoveNext(out ThreeMarketsDataProviderModel model);
            Assert.AreEqual(currentModel, model);
            Assert.AreEqual(currentModel, provider.Current);
        }

        [TestMethod]
        public void TestHistory()
        {
            provider.MoveNext(out _);
            provider.MoveNext(out _);
            Assert.AreEqual(3, provider.GetHistory(5).Count());

            provider.MoveNext(out _);
            provider.MoveNext(out _);
            provider.MoveNext(out _);
            provider.MoveNext(out _);
            provider.MoveNext(out _);
            provider.MoveNext(out ThreeMarketsDataProviderModel model);
            IEnumerable<ThreeMarketsDataProviderModel> models = provider.GetHistory(5);
            Assert.AreEqual(5, models.Count());
            Assert.AreEqual(model, models.ToArray()[4]);
        }
    }
}
