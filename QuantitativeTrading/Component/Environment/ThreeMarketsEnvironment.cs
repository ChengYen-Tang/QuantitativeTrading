using QuantitativeTrading.Component.DataProvider;
using QuantitativeTrading.Models;

namespace QuantitativeTrading.Component.Environment
{
    public class ThreeMarketsEnvironment : TradingEnvironment<ThreeMarketsDataProvider, ThreeMarketsDataProviderModel>, ISpot
    {
        public override decimal Assets 
            { get { return Balance + CoinBalance1 * CurrentKline.Coin12CoinKline.Close + CoinBalance2 * CurrentKline.Coin22CoinKline.Close; } }
        public decimal CoinBalance1 { get; private set; }
        public decimal CoinBalance2 { get; private set; }

        public ThreeMarketsEnvironment(ThreeMarketsDataProvider dataProvider, decimal cost)
            :base(dataProvider, cost)
        { }

        public (decimal balance, decimal CoinBalance1, decimal CoinBalance2) Trading(TradingAction action, TradingMarket market, int Count)
        {
            throw new System.NotImplementedException();
        }
    }

    public enum TradingMarket
    {
        Coin12Coin = 0,
        Coin22Coin,
        Coin22Coin1
    }
}
