using System;

namespace QuantitativeTrading.Models.Record
{
    public class ThreeMarketsModel
    {
        public DateTime Date { get; set; }
        public decimal Close { get; set; }
        public decimal Change { get; set; }
        public decimal Assets { get; set; }
        public decimal Balance { get; set; }
        public decimal CoinBalance1 { get; set; }
        public decimal CoinBalance2 { get; set; }
    }
    
}