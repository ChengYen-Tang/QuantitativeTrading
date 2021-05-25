using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantitativeTrading.Models;
using System.IO;
using System.Threading.Tasks;

namespace QuantitativeTrading.Tests.Models
{
    [TestClass]
    public class ThreeMarketsDataProviderModelTests
    {
        [TestMethod]
        public async Task TestLoadDataAsync()
        {
            ThreeMarketsModel model = await ThreeMarketsModel.CreateModel(Utils.btc_usdtPath, Utils.eth_usdtPath, Utils.eth_btcPath);
            Assert.AreEqual("BTCUSDT-Spot", model.Coin12CoinKlines[0].StockCode);
            Assert.AreEqual(309559, model.Coin12CoinKlines.Length);
            Assert.AreEqual("ETHUSDT-Spot", model.Coin22CoinKlines[0].StockCode);
            Assert.AreEqual(309559, model.Coin22CoinKlines.Length);
            Assert.AreEqual("ETHBTC-Spot", model.Coin22Coin1Klines[0].StockCode);
            Assert.AreEqual(309559, model.Coin22Coin1Klines.Length);
        }
    }
}
