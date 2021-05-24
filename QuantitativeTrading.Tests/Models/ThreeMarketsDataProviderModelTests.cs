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
            Assert.AreEqual("BTCUSDT-Spot", model.B2AKlines[0].StockCode);
            Assert.AreEqual(309559, model.B2AKlines.Length);
            Assert.AreEqual("ETHUSDT-Spot", model.C2AKlines[0].StockCode);
            Assert.AreEqual(309559, model.C2AKlines.Length);
            Assert.AreEqual("ETHBTC-Spot", model.C2BKlines[0].StockCode);
            Assert.AreEqual(309559, model.C2BKlines.Length);
        }
    }
}
