using MultilateralArbitrage.Models;

namespace MultilateralArbitrage.Modules.API
{
    internal interface IAPI
    {
        Task<ICollection<Symbol>> DownloadSymbolsAsync();
    }
}
