using System;
using System.Threading.Tasks;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models;
using QuantitativeTrading.Strategies.ThreeMarkets;
using ThreeMarketsRecordModel = QuantitativeTrading.Models.ThreeMarketsRecordModel;

namespace QuantitativeTrading.Runners.ThreeMarkets
{
    public class Runner<T>
        where T : Strategy
    {
        private readonly Recorder<ThreeMarketsRecordModel> recorder;
        private readonly SpotEnvironment environment;
        private readonly T strategy;

        public Runner(T strategy, SpotEnvironment environment, Recorder<ThreeMarketsRecordModel> recorder)
            => (this.strategy, this.environment, this.recorder) = (strategy, environment, recorder);

        public async Task RunAsync()
        {
            while (!environment.IsGameOver)
            {
                environment.MoveNextTime(out ThreeMarketsDataProviderModel data);
                StrategyAction action = strategy.PolicyDecision(data);
                Trading(action);
                Console.Clear();
                Console.WriteLine($"Date: {environment.CurrentKline.Coin12CoinKline.Date}, Assets: {environment.Assets}");

                recorder.Insert(new() { Date = data.Coin12CoinKline.Date, Coin12CoinClose = data.Coin12CoinKline.Close, Coin22CoinClose = data.Coin22CoinKline.Close, Coin22Coin1Close = data.Coin22Coin1Kline.Close, Assets = environment.Assets, Balance = environment.Balance, CoinBalance1 = environment.CoinBalance1, CoinBalance2 = environment.CoinBalance2, Coin1ToCoinChange = strategy.Coin1ToCoinChange, Coin2ToCoinChange = strategy.Coin2ToCoinChange });
            }

            await recorder.SaveAsync();
        }

        private void Trading(StrategyAction action)
        {
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
        }

        private void TwoStepTrading(TradingMarket source, TradingMarket target)
        {
            environment.Trading(TradingAction.Sell, source);
            environment.Trading(TradingAction.Buy, target);
        }
    }
}
