using MultilateralArbitrage.Models;

namespace MultilateralArbitrage.Modules.API
{
    internal interface IAPI
    {
        Task<ICollection<Symbol>> DownloadSymbolsAsync();
        Task<IDictionary<string, OrderBook>> GetAllOrderBooksAsync();
    }
}
