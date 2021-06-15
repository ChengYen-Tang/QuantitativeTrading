using System.Threading.Tasks;
using QuantitativeTrading.Environment;
using QuantitativeTrading.Models;
using QuantitativeTrading.Strategy.ThreeMarkets;
using ThreeMarketsModel = QuantitativeTrading.Models.Record.ThreeMarketsModel;

namespace QuantitativeTrading.Runner
{
    public class ThreeMarketsRunner<T>
        where T : Strategy.ThreeMarkets.Strategy, Strategy.ThreeMarkets.IStrategy
    {
        private readonly Recorder<ThreeMarketsModel> recorder;
        private readonly ThreeMarketsEnvironment environment;
        private readonly T strategy;

        public ThreeMarketsRunner(T strategy, ThreeMarketsEnvironment environment, Recorder<ThreeMarketsModel> recorder)
            => (this.strategy, this.environment, this.recorder) = (strategy, environment, recorder);

        public async Task RunAsync()
        {
            while(!environment.IsGameOver)
            {
                environment.MoveNextTime(out ThreeMarketsDataProviderModel data);
                StrategyAction action = strategy.PolicyDecision(data);
                if (action == StrategyAction.Coin)
                {
                    if (environment.CoinBalance1 > environment.Balance && environment.CoinBalance1 > environment.CoinBalance2)
                        environment.Trading(TradingAction.Sell, TradingMarket.Coin12Coin);
                    else if (environment.CoinBalance2 > environment.Balance && environment.CoinBalance2 > environment.CoinBalance1)
                        environment.Trading(TradingAction.Sell, TradingMarket.Coin22Coin);
                }
                else if (action == StrategyAction.Coin1)
                {
                    if (environment.Balance > environment.CoinBalance1 && environment.Balance > environment.CoinBalance2)
                        environment.Trading(TradingAction.Buy, TradingMarket.Coin12Coin);
                    else if (environment.CoinBalance2 > environment.CoinBalance1 && environment.Balance < environment.CoinBalance2)
                    {
                        if (strategy.BestCoin1ToCoin2Path(action) == BestPath.Path1)
                            TwoStepTrading(TradingMarket.Coin22Coin, TradingMarket.Coin12Coin);
                        else
                            environment.Trading(TradingAction.Sell, TradingMarket.Coin22Coin1);
                    }
                }
                else if (action == StrategyAction.Coin2)
                {
                    if (environment.Balance > environment.CoinBalance1 && environment.Balance > environment.CoinBalance2)
                        environment.Trading(TradingAction.Buy, TradingMarket.Coin22Coin);
                    else if (environment.CoinBalance2 < environment.CoinBalance1 && environment.Balance < environment.CoinBalance1)
                    {
                        if (strategy.BestCoin1ToCoin2Path(action) == BestPath.Path1)
                            TwoStepTrading(TradingMarket.Coin12Coin, TradingMarket.Coin22Coin);
                        else
                            environment.Trading(TradingAction.Buy, TradingMarket.Coin22Coin1);
                    }
                }

                recorder.Insert(new(){ Date = data.Coin12CoinKline.Date, Coin12CoinClose = data.Coin12CoinKline.Close, Coin22CoinClose = data.Coin22CoinKline.Close, Coin22Coin1Close = data.Coin22Coin1Kline.Close, Assets = environment.Assets, Balance = environment.Balance, CoinBalance1 = environment.CoinBalance1, CoinBalance2 = environment.CoinBalance2 });
            }

            await recorder.SaveAsync();
        }

        private void TwoStepTrading(TradingMarket source, TradingMarket target)
        {
            environment.Trading(TradingAction.Sell, source);
            environment.Trading(TradingAction.Buy, target);
        }
    }
}