using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Csv;
using System.Linq;
using System.Threading.Tasks;

namespace QuantitativeTrading.Models
{
    public class ThreeMarketsDataProviderModel
    {
        public KlineModel B2AKline { get; set; }
        public KlineModel C2AKline { get; set; }
        public KlineModel C2BKline { get; set; }
    }

    /// <summary>
    /// 歷史資料 CSV 路徑
    /// </summary>
    public class ThreeMarketsModel : DataProviderModel
    {
        public KlineModel[] B2AKlines { get; init; }
        public KlineModel[] C2AKlines { get; init; }
        public KlineModel[] C2BKlines { get; init; }

        private ThreeMarketsModel(KlineModel[] B2AKlines, KlineModel[] C2AKlines, KlineModel[] C2BKlines)
            => (this.B2AKlines, this.C2AKlines, this.C2BKlines) = (B2AKlines, C2AKlines, C2BKlines);

        /// <summary>
        /// 建立 ThreeMarketsDataProviderModel
        /// 
        /// 假設 A 是 USDT，B 是 BTC，C 是 ETH
        /// </summary>
        /// <param name="B2APath"> BTC 對 USDT 價格的路徑 (1 BTC = X USDT) </param>
        /// <param name="C2APath"> ETH 對 USDT 價格的路徑 (1 ETH = X USDT) </param>
        /// <param name="C2BPath"> ETH 對 BTC 價格的路徑 (1 ETH = X BTC) </param>
        /// <returns></returns>
        public static async Task<ThreeMarketsModel> CreateModel(string B2APath, string C2APath, string C2BPath)
        {
            Task<KlineModel[]>[] tasks = new[] { LoadCSVAsync(B2APath), LoadCSVAsync(C2APath), LoadCSVAsync(C2BPath) };
            KlineModel[][] klineModels = await Task.WhenAll(tasks);
            return new ThreeMarketsModel(klineModels[0], klineModels[1], klineModels[2]);
        }
    }

    public abstract class DataProviderModel
    {
        protected static async Task<KlineModel[]> LoadCSVAsync(string path)
        {
            IImporter importer = new CsvImporter();
            var result = await importer.Import<KlineModel>(path);
            if (result.HasError)
                throw result.Exception;

            return result.Data.OrderBy(item => item.Date).ToArray();
        }
    }
}
