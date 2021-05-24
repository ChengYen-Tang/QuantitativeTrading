using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantitativeTrading.Models;
using System.IO;
using System.Threading.Tasks;

namespace QuantitativeTrading.Tests.Models
{
    [TestClass]
    public class ThreeMarketsDataProviderModelTests
    {
        private static readonly string btc_usdtPath = Path.Combine(Utils.TestFilePath, "BTCUSDT-Spot.csv");
        private static readonly string eth_usdtPath = Path.Combine(Utils.TestFilePath, "ETHUSDT-Spot.csv");
        private static readonly string eth_btcPath = Path.Combine(Utils.TestFilePath, "ETHBTC-Spot.csv");

        [TestMethod]
        public async Task TestLoadDataAsync()
        {
            ThreeMarketsDataProviderModel model = await ThreeMarketsDataProviderModel.CreateModel(btc_usdtPath, eth_usdtPath, eth_btcPath);
            Assert.AreEqual("BTCUSDT-Spot", model.B2AKlines[0].StockCode);
            Assert.AreEqual(309559, model.B2AKlines.Length);
            Assert.AreEqual("ETHUSDT-Spot", model.C2AKlines[0].StockCode);
            Assert.AreEqual(309559, model.C2AKlines.Length);
            Assert.AreEqual("ETHBTC-Spot", model.C2BKlines[0].StockCode);
            Assert.AreEqual(309559, model.C2BKlines.Length);
        }
    }
}
