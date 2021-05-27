using QuantitativeTrading.Component.DataProvider;
using System;

namespace QuantitativeTrading.Component.Environment
{
    public abstract class TradingEnvironment<T, U> 
        where T : KlineDataProvider<U>
    {
        public abstract decimal Assets { get; }
        public bool IsGameOver { get { return Balance <= 0 || dataProvider.IsEnd; } }
        public decimal Balance { get; protected set; }
        public U CurrentKline { get { return dataProvider.Current; } }

        protected readonly decimal handlingFee;
        protected readonly T dataProvider;
        private readonly decimal smallestUnit;

        public TradingEnvironment(T dataProvider, decimal handlingFee, int smallestUnit)
            => (this.dataProvider, this.handlingFee, this.smallestUnit) = (dataProvider, handlingFee, Convert.ToDecimal(Math.Pow(10, smallestUnit)));

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
