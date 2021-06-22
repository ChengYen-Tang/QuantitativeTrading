using System;

namespace QuantitativeTrading.Models.Records.ThreeMarkets
{
    public interface IEnvironmentModels : Records.IEnvironmentModels
    {
        public DateTime Date { get; set; }
        public decimal Coin12CoinClose { get; set; }
        public decimal Coin22CoinClose { get; set; }
        public decimal Coin22Coin1Close { get; set; }
        public decimal Assets { get; set; }
        public decimal Balance { get; set; }
        public decimal CoinBalance1 { get; set; }
        public decimal CoinBalance2 { get; set; }
    }
}
