﻿using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records.ThreeMarkets;
using System.Runtime.CompilerServices;

namespace QuantitativeTrading.Environments.ThreeMarkets
{
    /// <summary>
    /// 現貨回測環境
    /// </summary>
    public class SpotEnvironment : Environment<ThreeMarketsDataProviderModel, ThreeMarketsDataProvider>
    {
        /// <summary>
        /// 資產
        /// </summary>
        public override decimal Assets => Balance + CoinBalance1 * CurrentKline.Coin12CoinKline.Close + CoinBalance2 * CurrentKline.Coin22CoinKline.Close;
        /// <summary>
        /// Coin1 的餘額
        /// </summary>
        public decimal CoinBalance1 { get; protected set; }
        /// <summary>
        /// Coin2 的餘額
        /// </summary>
        public decimal CoinBalance2 { get; protected set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="environmentParams"> 回測環境參數 </param>
        public SpotEnvironment(ThreeMarketsDataProvider dataProvider, EnvironmentParams environmentParams)
            : base(dataProvider, environmentParams) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="balance"> 初始餘額 </param>
        /// <param name="gameOverAssets"> 破產資產 </param>
        /// <param name="handlingFee"> 手續費% </param>
        /// <param name="smallestUnit"> 最小交易單位(小數點後幾位) </param>
        public SpotEnvironment(ThreeMarketsDataProvider dataProvider, decimal balance, decimal gameOverAssets, decimal handlingFee, int smallestUnit)
            : base(dataProvider, balance, gameOverAssets, handlingFee, smallestUnit) { }

        /// <summary>
        /// 交易
        /// </summary>
        /// <param name="action"> 動作(買/賣) </param>
        /// <param name="market"> 市場 (BTCUSDT, ETHUSDT, ETHBTC) </param>
        /// <returns></returns>
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

        /// <summary>
        /// 紀錄資料
        /// </summary>
        /// <param name="record"></param>
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

        /// <summary>
        /// 買的動作
        /// </summary>
        /// <param name="price"> 當前價錢 </param>
        /// <param name="balance"> 當前餘額 </param>
        /// <returns> (需要消耗多少成本, 能買多少單位) </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (decimal cost, decimal count) Buy(decimal price, decimal balance)
        {
            decimal buyCount = DecimalPointMask(balance / price);
            decimal handlingCost = buyCount * handlingFee;
            return (buyCount * price, DecimalPointMask(buyCount - handlingCost));
        }

        /// <summary>
        /// 賣的動作
        /// </summary>
        /// <param name="price"> 當前價錢 </param>
        /// <param name="balance"> 當前餘額 </param>
        /// <returns> (得到多少錢， 賣了多少單位) </returns>
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
