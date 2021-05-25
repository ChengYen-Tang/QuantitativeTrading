namespace QuantitativeTrading.Component.Environment
{
    public interface ISpot
    {
        public (decimal balance, decimal CoinBalance1, decimal CoinBalance2) Trading(TradingAction action, TradingMarket market, int Count);
    }
}
