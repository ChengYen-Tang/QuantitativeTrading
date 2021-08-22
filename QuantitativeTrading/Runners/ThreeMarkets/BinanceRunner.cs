using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Strategies.ThreeMarkets;
using System;
using System.Threading;
using System.Threading.Tasks;
using IEnvironmentModels = QuantitativeTrading.Models.Records.ThreeMarkets.IEnvironmentModels;

namespace QuantitativeTrading.Runners.ThreeMarkets
{
    public class BinanceRunner<T, U> : Runner<T, U>, IDisposable
        where T : Strategy
        where U : class, IEnvironmentModels, IStrategyModels, new()
    {
        private readonly BinanceSpot env;
        private readonly CancellationTokenSource cancellationTokenSource;
        private bool isRun;
        private Task runTask;
        private bool disposedValue;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="strategy"> 策略 </param>
        /// <param name="environment"> 回測環境 </param>
        /// <param name="recorder"> 交易紀錄器 </param>
        public BinanceRunner(T strategy, BinanceSpot environment, Recorder<U> recorder)
            : base(strategy, environment, recorder)
            => (env, isRun, cancellationTokenSource) = (environment, false, new());

        public override Task RunAsync()
        {
            if (isRun)
                return Task.CompletedTask;
            env.Run();
            runTask = Task.Factory.StartNew(Worker, TaskCreationOptions.LongRunning);
            isRun = true;
            return Task.CompletedTask;
        }

        private async Task Worker()
        {
            CancellationToken cancellationToken= cancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                ThreeMarketsDataProviderModel data = await env.GetKlineAsync();
                StrategyAction action = strategy.PolicyDecision(data);
                await env.ReflashAcountInfo();
                Trading(action);
                if (recorder is not null)
                {
                    U record = new();
                    environment.Recording(record);
                    strategy.Recording(record);
                    recorder.Insert(record);
                    await recorder.SaveAsync();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                    cancellationTokenSource.Cancel();
                    runTask.Wait();
                    env.ReflashAcountInfo().Wait();
                    Trading(StrategyAction.Coin);
                    env.Dispose();
                    runTask.Dispose();
                    cancellationTokenSource.Dispose();
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        ~BinanceRunner()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
