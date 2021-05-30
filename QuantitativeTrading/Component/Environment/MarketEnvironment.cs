using QuantitativeTrading.Component.DataProvider;
using System;

namespace QuantitativeTrading.Component.Environment
{
    public abstract class MarketEnvironment<T, U> 
        where T : KlineDataProvider<U>
    {
        public abstract decimal Assets { get; }
        public bool IsGameOver { get { return Assets <= gameOverAssets || dataProvider.IsEnd; } }
        public decimal Balance { get; protected set; }
        public U CurrentKline { get { return dataProvider.Current; } }

        protected readonly decimal handlingFee;
        protected readonly T dataProvider;
        private readonly decimal gameOverAssets;
        private readonly decimal smallestUnit;

        public MarketEnvironment(T dataProvider, decimal balance, decimal gameOverAssets, decimal handlingFee, int smallestUnit)
            => (this.dataProvider, Balance, this.gameOverAssets, this.handlingFee, this.smallestUnit) = (dataProvider, balance, gameOverAssets, handlingFee / 100, Convert.ToDecimal(Math.Pow(10, smallestUnit)));

        public bool MoveNextTime(out U model)
            => dataProvider.MoveNext(out model);

        protected decimal DecimalPointMask(decimal d)
            => Math.Floor(d * smallestUnit) / smallestUnit;
    }

    public enum TradingAction
    {
        Buy = 0,
        Sell
    }
}
