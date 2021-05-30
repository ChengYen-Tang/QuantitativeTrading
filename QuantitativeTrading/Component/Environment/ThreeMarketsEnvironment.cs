using QuantitativeTrading.Component.DataProvider;
using QuantitativeTrading.Models;

namespace QuantitativeTrading.Component.Environment
{
    public class ThreeMarketsEnvironment : MarketEnvironment<ThreeMarketsDataProvider, ThreeMarketsDataProviderModel>, ISpot
    {
        public override decimal Assets 
            { get { return Balance + CoinBalance1 * CurrentKline.Coin12CoinKline.Close + CoinBalance2 * CurrentKline.Coin22CoinKline.Close; } }
        public decimal CoinBalance1 { get; private set; }
        public decimal CoinBalance2 { get; private set; }

        public ThreeMarketsEnvironment(ThreeMarketsDataProvider dataProvider, decimal balance, decimal gameOverAssets, decimal handlingFee, int smallestUnit)
            :base(dataProvider, balance, gameOverAssets, handlingFee, smallestUnit)
        { }

        public (decimal balance, decimal CoinBalance1, decimal CoinBalance2) Trading(TradingAction action, TradingMarket market)
        {
            if (market == TradingMarket.Coin12Coin)
                (Balance, CoinBalance1) = TradingAction(action, dataProvider.Current.Coin12CoinKline.Close, Balance, CoinBalance1);
            if (market == TradingMarket.Coin22Coin)
                (Balance, CoinBalance2) = TradingAction(action, dataProvider.Current.Coin22CoinKline.Close, Balance, CoinBalance2);
            if (market == TradingMarket.Coin22Coin1)
                (CoinBalance1, CoinBalance2) = TradingAction(action, dataProvider.Current.Coin22Coin1Kline.Close, CoinBalance1, CoinBalance2);

            return (Balance, CoinBalance1, CoinBalance2);
        }

        private (decimal mainBalance, decimal secondaryBalance) TradingAction(TradingAction action, decimal price, decimal mainBalance, decimal secondaryBalance)
        {
            if (action == Environment.TradingAction.Buy)
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

        private (decimal cost, decimal count) Buy(decimal price, decimal balance)
        {
            decimal buyCount = DecimalPointMask(balance / price);
            decimal handlingCost = buyCount * handlingFee;
            return (buyCount * price, DecimalPointMask(buyCount - handlingCost));
        }

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
