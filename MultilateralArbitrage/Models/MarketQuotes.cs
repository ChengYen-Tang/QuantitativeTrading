using System;

namespace MultilateralArbitrage.Models
{
    internal record OrderBook(decimal BidPrice, decimal BidQuantity, decimal AskPrice, decimal AskQuantity, DateTime? Timestamp);
}
