using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Csv;
using MoreLinq.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace QuantitativeTrading.Models
{
    public class ThreeMarketsDataProviderModel
    {
        public KlineModel Coin12CoinKline { get; set; }
        public KlineModel Coin22CoinKline { get; set; }
        public KlineModel Coin22Coin1Kline { get; set; }
    }

    /// <summary>
    /// 歷史資料 CSV 路徑
    /// </summary>
    public class ThreeMarketsModel : DataProviderModel
    {
        public KlineModel[] Coin12CoinKlines { get; init; }
        public KlineModel[] Coin22CoinKlines { get; init; }
        public KlineModel[] Coin22Coin1Klines { get; init; }

        public ThreeMarketsModel(KlineModel[] Coin12CoinKlines, KlineModel[] Coin22CoinKlines, KlineModel[] Coin22Coin1Klines)
            => (this.Coin12CoinKlines, this.Coin22CoinKlines, this.Coin22Coin1Klines) = (Coin12CoinKlines, Coin22CoinKlines, Coin22Coin1Klines);

        /// <summary>
        /// 建立 ThreeMarketsDataProviderModel
        /// 
        /// 假設 Coin 是 USDT，Coin1 是 BTC，Coin2 是 ETH
        /// </summary>
        /// <param name="Coin12CoinPath"> BTC 對 USDT 價格的路徑 (1 BTC = X USDT) </param>
        /// <param name="Coin22CoinPath"> ETH 對 USDT 價格的路徑 (1 ETH = X USDT) </param>
        /// <param name="Coin22Coin1Path"> ETH 對 BTC 價格的路徑 (1 ETH = X BTC) </param>
        /// <returns></returns>
        public static async Task<ThreeMarketsModel> CreateModel(string Coin12CoinPath, string Coin22CoinPath, string Coin22Coin1Path)
        {
            Task<KlineModel[]>[] tasks = new[] { LoadCSVAsync(Coin12CoinPath), LoadCSVAsync(Coin22CoinPath), LoadCSVAsync(Coin22Coin1Path) };
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

            return result.Data.DistinctBy(item => item.Date).OrderBy(item => item.Date).ToArray();
        }
    }
}
