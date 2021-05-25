using QuantitativeTrading.Component.DataProvider;

namespace QuantitativeTrading.Component.Environment
{
    public abstract class TradingEnvironment<T, U> 
        where T : KlineDataProvider<U>
    {
        public abstract decimal Assets { get; }
        public bool IsGameOver { get { return Balance <= 0 || dataProvider.IsEnd; } }
        public decimal Balance { get; private set; }
        public U CurrentKline { get { return dataProvider.Current; } }

        protected readonly decimal cost;
        protected readonly T dataProvider;

        public TradingEnvironment(T dataProvider, decimal cost)
            => (this.dataProvider, this.cost) = (dataProvider, cost);

        public bool MoveNextTime(out U model)
            => dataProvider.MoveNext(out model);
    }

    public enum TradingAction
    {
        Buy = 0,
        Sell
    }
}
