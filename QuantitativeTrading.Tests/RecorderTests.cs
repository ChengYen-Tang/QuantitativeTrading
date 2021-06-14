using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Csv;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantitativeTrading.Models.Record;

namespace QuantitativeTrading.Tests
{
    [TestClass]
    public class RecorderTests
    {
        [TestMethod]
        public async Task TestRecordAsync()
        {
            Recorder<ThreeMarketsModel> recorder = new("Test", AppDomain.CurrentDomain.BaseDirectory);
            DateTime date = DateTime.Now;
            recorder.Insert(new ThreeMarketsModel{ Close = 1, Balance = 2, Assets = 3, Date = date });
            recorder.Insert(new ThreeMarketsModel{ Close = 4, Balance = 5, Assets = 6, Date = date });
            await recorder.SaveAsync();

            IImporter importer = new CsvImporter();
            var result = (await importer.Import<ThreeMarketsModel>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test.csv"))).Data.ToList();
            Assert.AreEqual(date.ToString(), result[0].Date.ToString());
            Assert.AreEqual(1, result[0].Close);
            Assert.AreEqual(2, result[0].Balance);
            Assert.AreEqual(3, result[0].Assets);
            Assert.AreEqual(date.ToString(), result[1].Date.ToString());
            Assert.AreEqual(4, result[1].Close);
            Assert.AreEqual(5, result[1].Balance);
            Assert.AreEqual(6, result[1].Assets);
        }
    }
}