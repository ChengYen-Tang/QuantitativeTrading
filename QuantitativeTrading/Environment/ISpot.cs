namespace QuantitativeTrading.Environment
{
    public interface ISpot
    {
        public (decimal balance, decimal CoinBalance1, decimal CoinBalance2) Trading(TradingAction action, TradingMarket market);
    }
}
