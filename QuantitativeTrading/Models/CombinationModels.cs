﻿using Magicodes.ExporterAndImporter.Core;
using System;

namespace QuantitativeTrading.Models
{
    public class CloseChangeSumCombinationModels
    {
        public string Combination { get; set; }
        public decimal Assets { get; set; }
        public decimal Balance { get; set; }
        public decimal CoinBalance1 { get; set; }
        public decimal CoinBalance2 { get; set; }
        [ExporterHeader(Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime EndDate { get; set; }
    }
}
