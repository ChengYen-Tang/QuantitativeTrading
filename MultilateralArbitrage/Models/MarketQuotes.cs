using System;

namespace MultilateralArbitrage.Models
{
    internal record Order(Symbol Symbol, bool IsBuy, bool IsImmediatelyMatch, decimal Price);
    internal record OrderBook(decimal BidPrice, decimal BidQuantity, decimal AskPrice, decimal AskQuantity, DateTime? Timestamp);
    internal record LatestPrice(decimal Price, DateTime? Timestamp);
}
