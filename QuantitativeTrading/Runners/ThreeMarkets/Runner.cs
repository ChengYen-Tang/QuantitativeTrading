using System;
using System.Threading.Tasks;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Strategies.ThreeMarkets;
using IEnvironmentModels = QuantitativeTrading.Models.Records.ThreeMarkets.IEnvironmentModels;

namespace QuantitativeTrading.Runners.ThreeMarkets
{
    public class Runner<T, U>
        where T : Strategy
        where U : class, IEnvironmentModels, IStrategyModels, new()
    {
        private readonly int consoleLineTop;
        private readonly Recorder<U> recorder;
        private readonly SpotEnvironment environment;
        private readonly T strategy;

        public Runner(T strategy, SpotEnvironment environment, Recorder<U> recorder, int? consoleLineTop = null)
        {
            (this.strategy, this.environment, this.recorder) = (strategy, environment, recorder);
            this.consoleLineTop = consoleLineTop ??= Console.GetCursorPosition().Top;
        }

        public async Task RunAsync()
        {
            while (!environment.IsGameOver)
            {
                ThreeMarketsDataProviderModel data = environment.CurrentKline;
                StrategyAction action = strategy.PolicyDecision(data);
                Trading(action);
                Console.SetCursorPosition(0, consoleLineTop);
                Console.WriteLine($"Date: {environment.CurrentKline.Coin12CoinKline.Date}, Assets: {environment.Assets}");

                U record = new();
                environment.Recording(record);
                strategy.Recording(record);
                recorder.Insert(record);
                environment.MoveNextTime(out _);
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
