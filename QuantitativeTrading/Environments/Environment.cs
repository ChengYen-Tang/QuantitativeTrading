using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Models.Records;
using System;
using System.Runtime.CompilerServices;

namespace QuantitativeTrading.Environments
{
    public abstract class Environment<T, U>
        where U : KlineDataProvider<T, U>
    {
        public abstract decimal Assets { get; }
        public bool IsGameOver => Assets <= gameOverAssets || dataProvider.IsEnd;
        public decimal Balance { get; protected set; }
        public T CurrentKline => dataProvider.Current;

        protected readonly decimal handlingFee;
        protected readonly U dataProvider;
        private readonly decimal gameOverAssets;
        private readonly decimal smallestUnit;

        public Environment(U dataProvider, decimal balance, decimal gameOverAssets, decimal handlingFee, int smallestUnit)
            => (this.dataProvider, Balance, this.gameOverAssets, this.handlingFee, this.smallestUnit) = (dataProvider, balance, gameOverAssets, handlingFee / 100, Convert.ToDecimal(Math.Pow(10, smallestUnit)));

        public bool MoveNextTime(out T model)
            => dataProvider.MoveNext(out model);

        public abstract void Recording(IEnvironmentModels record);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected decimal DecimalPointMask(decimal d)
            => Math.Floor(d * smallestUnit) / smallestUnit;
    }

    public enum TradingAction
    {
        Buy = 0,
        Sell
    }
}
