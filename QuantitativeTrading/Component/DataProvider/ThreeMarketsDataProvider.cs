using QuantitativeTrading.Models;
using System.Collections.Generic;
using System.Linq;

namespace QuantitativeTrading.Component.DataProvider
{
    public class ThreeMarketsDataProvider : DataProvider<ThreeMarketsDataProviderModel>
    {
        public ThreeMarketsDataProvider(ThreeMarketsModel model)
            => models = Join(model);

        private static List<ThreeMarketsDataProviderModel> Join(ThreeMarketsModel model)
            => model.B2AKlines.AsParallel()
            .Join(model.C2AKlines.AsParallel(), B2AKline => B2AKline.Date, C2AKline => C2AKline.Date, (B2AKline, C2AKline) => new { B2AKline, C2AKline })
            .Join(model.C2BKlines.AsParallel(), item => item.B2AKline.Date, C2BKline => C2BKline.Date, (item, C2BKline) => new ThreeMarketsDataProviderModel { B2AKline = item.B2AKline, C2AKline = item.C2AKline, C2BKline = C2BKline })
            .OrderBy(item => item.B2AKline.Date).ToList();
    }
}
