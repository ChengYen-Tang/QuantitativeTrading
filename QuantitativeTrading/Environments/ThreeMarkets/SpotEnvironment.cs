using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records.ThreeMarkets;
using System.Runtime.CompilerServices;

namespace QuantitativeTrading.Environments.ThreeMarkets
{
    public class SpotEnvironment : Environment<ThreeMarketsDataProvider, ThreeMarketsDataProviderModel>
    {
        public override decimal Assets
        { get { return Balance + CoinBalance1 * CurrentKline.Coin12CoinKline.Close + CoinBalance2 * CurrentKline.Coin22CoinKline.Close; } }
        public decimal CoinBalance1 { get; protected set; }
        public decimal CoinBalance2 { get; protected set; }

        public SpotEnvironment(ThreeMarketsDataProvider dataProvider, decimal balance, decimal gameOverAssets, decimal handlingFee, int smallestUnit)
            : base(dataProvider, balance, gameOverAssets, handlingFee, smallestUnit)
        { }

        public virtual (decimal balance, decimal CoinBalance1, decimal CoinBalance2) Trading(TradingAction action, TradingMarket market)
        {
            if (market == TradingMarket.Coin12Coin)
                (Balance, CoinBalance1) = TradingAction(action, dataProvider.Current.Coin12CoinKline.Close, Balance, CoinBalance1);
            if (market == TradingMarket.Coin22Coin)
                (Balance, CoinBalance2) = TradingAction(action, dataProvider.Current.Coin22CoinKline.Close, Balance, CoinBalance2);
            if (market == TradingMarket.Coin22Coin1)
                (CoinBalance1, CoinBalance2) = TradingAction(action, dataProvider.Current.Coin22Coin1Kline.Close, CoinBalance1, CoinBalance2);

            return (Balance, CoinBalance1, CoinBalance2);
        }

        public override void Recording(Models.Records.IEnvironmentModels record)
        {
            IEnvironmentModels spotRecord = record as IEnvironmentModels;
            spotRecord.Assets = Assets;
            spotRecord.Balance = Balance;
            spotRecord.CoinBalance1 = CoinBalance1;
            spotRecord.CoinBalance2 = CoinBalance2;
            spotRecord.Date = CurrentKline.Coin12CoinKline.Date;
            spotRecord.Coin12CoinClose = CurrentKline.Coin12CoinKline.Close;
            spotRecord.Coin22CoinClose = CurrentKline.Coin22CoinKline.Close;
            spotRecord.Coin22Coin1Close = CurrentKline.Coin22Coin1Kline.Close;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (decimal mainBalance, decimal secondaryBalance) TradingAction(TradingAction action, decimal price, decimal mainBalance, decimal secondaryBalance)
        {
            if (action == Environments.TradingAction.Buy)
            {
                (decimal cost, decimal count) = Buy(price, mainBalance);
                return (mainBalance - cost, secondaryBalance + count);
            }
            else
            {
                (decimal income, decimal count) = Sell(price, secondaryBalance);
                return (mainBalance + income, secondaryBalance - count);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (decimal cost, decimal count) Buy(decimal price, decimal balance)
        {
            decimal buyCount = DecimalPointMask(balance / price);
            decimal handlingCost = buyCount * handlingFee;
            return (buyCount * price, DecimalPointMask(buyCount - handlingCost));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (decimal income, decimal count) Sell(decimal price, decimal balance)
        {
            decimal count = DecimalPointMask(balance);
            decimal sellIncome = count * price;
            decimal handlingCost = sellIncome * handlingFee;
            return (sellIncome - handlingCost, count);
        }
    }

    public enum TradingMarket
    {
        Coin12Coin = 0,
        Coin22Coin,
        Coin22Coin1
    }
}
