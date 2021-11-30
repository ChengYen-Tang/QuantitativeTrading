using MultilateralArbitrage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultilateralArbitrage.Modules.API
{
    internal interface IAPI
    {
        Task<ICollection<Symbol>> DownloadSymbolsAsync();
        Task<IDictionary<string, OrderBook>> GetAllOrderBooksAsync();
        Task<IDictionary<string, LatestPrice>> GetAllLatestPrice();
    }
}
