using Magicodes.ExporterAndImporter.Core;
using System;

namespace QuantitativeTrading.Models.Records.ThreeMarkets
{
    public class CloseChangeRecordModel : IEnvironmentModels, ICloseChangeModels
    {
        [ExporterHeader(Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Date { get; set; }
        public decimal Coin12CoinClose { get; set; }
        public decimal Coin22CoinClose { get; set; }
        public decimal Coin22Coin1Close { get; set; }
        public decimal Assets { get; set; }
        public decimal Balance { get; set; }
        public decimal CoinBalance1 { get; set; }
        public decimal CoinBalance2 { get; set; }
        public decimal Coin1ToCoinChangeSum { get; set; }
        public decimal Coin2ToCoinChangeSum { get; set; }
    }

    public class AutoParamsCloseChangeRecordModel : IEnvironmentModels, IAutoParamsCloseChange
    {
        [ExporterHeader(Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Date { get; set; }
        public decimal Coin12CoinClose { get; set; }
        public decimal Coin22CoinClose { get; set; }
        public decimal Coin22Coin1Close { get; set; }
        public decimal Assets { get; set; }
        public decimal Balance { get; set; }
        public decimal CoinBalance1 { get; set; }
        public decimal CoinBalance2 { get; set; }
        public decimal Coin1ToCoinChangeSum { get; set; }
        public decimal Coin2ToCoinChangeSum { get; set; }
        public int ObservationTime { get; set; }
        public int TradingInterval { get; set; }
    }
}
