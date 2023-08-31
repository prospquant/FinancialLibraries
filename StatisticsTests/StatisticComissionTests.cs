using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FinancialStatistics;
using FinancialStatistics.Models;
using HQConnector.Dto.DTO.Commission;
using HQConnector.Dto.DTO.Commission.Model;
using HQConnector.Dto.DTO.Enums.Commission;
using HQConnector.Dto.DTO.Enums.Exchange;
using HQConnector.Dto.DTO.Enums.MyTrade;
using HQConnector.Dto.DTO.Enums.Orders;
using HQConnector.Dto.DTO.Trade;
using Xunit;

namespace StatisticsTests
{
    public class StatisticComissionTests
    {
        [Fact]
        public async Task TryCreateDeal()
        {
            var deal = new Deal("BTCUSDT",Sides.Buy,new DateTime(2021,11,06),new DateTime(2021,11,06),60000,61000,1,0, 1000,1000,1000);
            Assert.NotNull(deal);
          
        }

        #region CalculateTillZeroMethodTests

        [Fact]
        public async Task TryCalculateDealbyTillZeroMethodProfit()
        {
            
        
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 2, null), null)

            };
            Assert.NotNull(firsttrade);
            var secondtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 62000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 62000, 2, null), null)
            };
            Assert.NotNull(firsttrade);
            var deal = CalculateDeal.CalculateDealsByTillZeroMethod(new List<MyTrade>() { firsttrade, secondtrade });
            Assert.NotNull(deal.Data);
            Assert.True(deal.Data.FormedDeals.Count() != 0);
        }

        [Fact]
        public async Task TryCalculateDealbyTillZeroMethodProfitReverse()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 2, null), null)
            };
            Assert.NotNull(firsttrade);
            var secondtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 2, null), null)
            };
            Assert.NotNull(secondtrade);
            var deal = CalculateDeal.CalculateDealsByTillZeroMethod(new List<MyTrade>() { firsttrade,secondtrade });
            Assert.NotNull(deal.Data);
            Assert.True(deal.Data.FormedDeals.Count() != 0);
        }

        [Fact]
        public async Task TryCalculateDealbyTillZeroMethodLose()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2.5m,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 2.5m, null), null)
            };
            Assert.NotNull(firsttrade);
            var secondtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 57000,
                Amount = 2.5m,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 57000, 2.5m, null), null)
            };
            Assert.NotNull(secondtrade);
            var deal = CalculateDeal.CalculateDealsByTillZeroMethod(new List<MyTrade>() { firsttrade, secondtrade });
            Assert.NotNull(deal);
            Assert.True(deal.Data.FormedDeals.Count() != 0);
        }

        [Fact]
        public async Task TryCalculateDealbyTillZeroMethodLoseReverse()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 1.5m,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 1.5m, null), null)
            };
            Assert.NotNull(firsttrade);
            var secondtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 62500,
                Amount = 1.5m,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 62500, 1.5m, null), null)
            };
            Assert.NotNull(secondtrade);
            var deal = CalculateDeal.CalculateDealsByTillZeroMethod(new List<MyTrade>() { firsttrade, secondtrade });
            Assert.NotNull(deal);
            Assert.True(deal.Data.FormedDeals.Count() != 0);
        }

       
     
        #endregion

        #region CalculateFIFOMethodTests
        [Fact]
        public async Task TryCalculateDealbyFIFOMethodProfit()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 2, null), null)
            };
            Assert.NotNull(firsttrade);

            var secondtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 62000,
                Amount = 2,
                Commission= new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 62000, 2, null), null)
            };
            Assert.NotNull(secondtrade);

            var deal = CalculateDeal.CalculateDealsByFIFOMethod(new List<MyTrade>() { firsttrade, secondtrade });
            Assert.NotNull(deal.Data);
            Assert.True(deal.Data.FormedDeals.Count() != 0);
        }

        [Fact]
        public async Task TryCalculateDealbyFIFOMethodProfitReverse()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 2, null), null)
            };
            Assert.NotNull(firsttrade);

            var secondtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 2, null), null)
            };
            Assert.NotNull(secondtrade);

            var deal = CalculateDeal.CalculateDealsByFIFOMethod(new List<MyTrade>() { firsttrade, secondtrade });
            Assert.NotNull(deal.Data);
            Assert.True(deal.Data.FormedDeals.Count() != 0);
        }

        [Fact]
        public async Task TryCalculateDealbyFIFOMethodLose()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2.5m,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 2.5m, null), null)
            };
            Assert.NotNull(firsttrade);

            var secondtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 57000,
                Amount = 2.5m,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 57000, 2.5m, null), null)
            };
            Assert.NotNull(secondtrade);

            var deal = CalculateDeal.CalculateDealsByFIFOMethod(new List<MyTrade>() { firsttrade, secondtrade });
            Assert.NotNull(deal.Data);
            Assert.True(deal.Data.FormedDeals.Count() != 0);
        }

        [Fact]
        public async Task TryCalculateDealbyFIFOMethodLoseReverse()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 1.5m,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 1.5m, null), null)
            };
            Assert.NotNull(firsttrade);

            var secondtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 62500,
                Amount = 1.5m,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 62500, 1.5m, null), null)
            };
            Assert.NotNull(secondtrade);

            var deal = CalculateDeal.CalculateDealsByFIFOMethod(new List<MyTrade>() { firsttrade, secondtrade });
            Assert.NotNull(deal.Data);
            Assert.True(deal.Data.FormedDeals.Count() != 0);
        }

    
      
    
        #endregion

        
        #region EquityCurveTests
        [Fact]
        public async Task TryCalculateCurveEquitySimply()
        {
            var firsttrade = new MyTrade()
            {
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 2, null), null)
            };
            Assert.NotNull(firsttrade);

            var secondtrade = new MyTrade()
            {
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 62000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 62000, 2, null), null)
            };
            Assert.NotNull(secondtrade);

            var thirdtrade = new MyTrade()
            {
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 2, null), null)
            };
            Assert.NotNull(thirdtrade);

            var fourtrade = new MyTrade()
            {
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 2, null), null)
            };
            Assert.NotNull(fourtrade);

            var deal = CalculateDeal.CalculateDealsByFIFOMethod(new List<MyTrade>() { firsttrade, secondtrade, thirdtrade, fourtrade });
            Assert.NotNull(deal);
            Assert.True(deal.Data.FormedDeals.Count() != 0);

            var curve = CalculateEquityCurve.CalculateEquity(deal.Data, 100000);
            Assert.NotNull(curve);
        }

        [Fact]
        public async Task TryCalculateCurveEquityFromSample()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 1m, null), null)
            };
            Assert.NotNull(firsttrade);

            var secondtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 60920,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60920, 1m, null), null)
            };
            Assert.NotNull(secondtrade);

            var thirdtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 12, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 1m, null), null)
            };
            Assert.NotNull(thirdtrade);

            var fourtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 13, 10, 00, TimeSpan.Zero),
                Price = 59020,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 59020, 1m, null), null)
            };
            Assert.NotNull(fourtrade);

            var fifthtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 14, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 1m, null), null)
            };
            Assert.NotNull(fifthtrade);

            var sixtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 15, 10, 00, TimeSpan.Zero),
                Price = 59480,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 59480, 1m, null), null)
            };
            Assert.NotNull(sixtrade);

            var seventrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 16, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 1m, null), null)
            };
            Assert.NotNull(seventrade);

            var eighttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 17, 10, 00, TimeSpan.Zero),
                Price = 57500,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 57500, 1m, null), null)
            };
            Assert.NotNull(eighttrade);

            var ninetrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 18, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 1m, null), null)
            };
            Assert.NotNull(ninetrade);

            var tentrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 19, 10, 00, TimeSpan.Zero),
                Price = 58450,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58450, 1m, null), null)
            };
            Assert.NotNull(tentrade);

            var dealfifo = CalculateDeal.CalculateDealsByFIFOMethod(new List<MyTrade>() { firsttrade, secondtrade, thirdtrade, fourtrade,
                fifthtrade,sixtrade,seventrade,eighttrade,
            ninetrade,tentrade});

            Assert.NotNull(dealfifo);
            Assert.True(dealfifo.Data.FormedDeals.Count() != 0);

            var curvefifo = CalculateEquityCurve.CalculateEquity(dealfifo.Data, 100000);
            Assert.NotNull(curvefifo);

            var dealtillzero = CalculateDeal.CalculateDealsByTillZeroMethod(new List<MyTrade>() { firsttrade, secondtrade, thirdtrade, fourtrade,
                fifthtrade,sixtrade,seventrade,eighttrade,
            ninetrade,tentrade});

            Assert.NotNull(dealtillzero);
            Assert.True(dealtillzero.Data.FormedDeals.Count() != 0);

            var curvetillzero = CalculateEquityCurve.CalculateEquity(dealtillzero.Data, 100000);
            Assert.NotNull(curvetillzero);
            Assert.Equal(curvefifo.Data.EquityCurve.Last(), curvetillzero.Data.EquityCurve.Last());
        }

        [Fact]
        public async Task TryCalculateCurveEquityFromSampleReverse()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000, 1m, null), null)
            };
            Assert.NotNull(firsttrade);

            var secondtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 11, 00, 00, TimeSpan.Zero),
                Price = 60920,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60920, 1m, null), null)

            };
            Assert.NotNull(secondtrade);

            var thirdtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 12, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 1m, null), null)

            };
            Assert.NotNull(thirdtrade);

            var fourtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 13, 10, 00, TimeSpan.Zero),
                Price = 59020,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 59020, 1m, null), null)

            };
            Assert.NotNull(fourtrade);

            var fifthtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 14, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 1m, null), null)

            };
            Assert.NotNull(fifthtrade);

            var sixtrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 15, 10, 00, TimeSpan.Zero),
                Price = 59480,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 59480, 1m, null), null)

            };
            Assert.NotNull(sixtrade);

            var seventrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 16, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 1m, null), null)

            };
            Assert.NotNull(seventrade);

            var eighttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 17, 10, 00, TimeSpan.Zero),
                Price = 57500,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 57500, 1m, null), null)

            };
            Assert.NotNull(eighttrade);

            var ninetrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 18, 00, 00, TimeSpan.Zero),
                Price = 58000,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58000, 1m, null), null)

            };
            Assert.NotNull(ninetrade);

            var tentrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Sell,
                Time = new DateTimeOffset(2021, 11, 06, 19, 10, 00, TimeSpan.Zero),
                Price = 58450,
                Amount = 1,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 58450, 1m, null), null)

            };
            Assert.NotNull(tentrade);

            var dealfifo = CalculateDeal.CalculateDealsByFIFOMethod(new List<MyTrade>() { firsttrade, secondtrade, thirdtrade, fourtrade,
                fifthtrade,sixtrade,seventrade,eighttrade,
            ninetrade,tentrade});

            Assert.NotNull(dealfifo);
            Assert.True(dealfifo.Data.FormedDeals.Count() != 0);

            var curvefifo = CalculateEquityCurve.CalculateEquity(dealfifo.Data, 100000);
            Assert.NotNull(curvefifo);

            var dealtillzero = CalculateDeal.CalculateDealsByTillZeroMethod(new List<MyTrade>() { firsttrade, secondtrade, thirdtrade, fourtrade,
                fifthtrade,sixtrade,seventrade,eighttrade,
            ninetrade,tentrade});
            Assert.NotNull(dealtillzero);
            Assert.True(dealtillzero.Data.FormedDeals.Count() != 0);

            var curvetillzero = CalculateEquityCurve.CalculateEquity(dealtillzero.Data, 100000);
            Assert.NotNull(curvetillzero);
            Assert.Equal(curvefifo.Data.EquityCurve.Last(), curvetillzero.Data.EquityCurve.Last());
        }
        #endregion

        #region CommissionTests

        [Fact]
        public async Task TryCreateCommission()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.Binance,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance,null,CommissionHelper.CalculateCommissionOnCommissionRate(Exchange.Binance, 60000,2,null),"")
            };
            Assert.NotNull(firsttrade);
            Assert.True(firsttrade.Commission.CurrentComissionValue > 0);

        }

        [Fact]
        public async Task TryCreateCommissionByDefaultCommissionRate()
        {
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.BinanceFutures,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.Binance, null, CommissionHelper.CalculateCommissionOnCommissionRate(CommissionHelper.ReturnCommissionRateByExchange(Exchange.BinanceFutures), 60000, 2, null), "")
            };
            Assert.NotNull(firsttrade);
            Assert.True(firsttrade.Commission.CurrentComissionValue > 0);

        }

        [Fact]
        public async Task TryMakeCommissionRate()
        {
            var conditionRateGrid= new CommissionRateGrid();
            conditionRateGrid.Add(-1,new CommissionAdditionalCondition(TakerOrMaker.Taker,0.04M));
            conditionRateGrid.Add(1,new CommissionAdditionalCondition(TakerOrMaker.Maker, 0.02M));
            var commissionRate = new CommissionRateModel(ComissionChargingType.PercentageGrid, conditionRateGrid);
            var firsttrade = new MyTrade()
            {
                Exchange = HQConnector.Dto.DTO.Enums.Exchange.Exchange.BinanceFutures,
                Pair = "BTCUSDT",
                Side = Sides.Buy,
                Time = new DateTimeOffset(2021, 11, 06, 10, 00, 00, TimeSpan.Zero),
                Price = 60000,
                Amount = 2,
                Commission = new CommissionModel(Exchange.BinanceFutures, commissionRate, CommissionHelper.CalculateCommissionOnCommissionRate(commissionRate, 60000, 2, TakerOrMaker.Maker), "")
            };
            Assert.NotNull(firsttrade);
            Assert.True(firsttrade.Commission.CurrentComissionValue > 0);

        }
        #endregion
    }
}
