using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Csv;
using MoreLinq.Extensions;
using QuantitativeTrading.Models;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace QuantitativeTrading.Data.DataLoaders
{
    public class KlineDataLoader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
