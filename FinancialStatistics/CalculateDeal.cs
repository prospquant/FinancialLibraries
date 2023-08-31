using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HQConnector.Dto.DTO.Commission;
using HQConnector.Dto.DTO.Commission.Model;
using HQConnector.Dto.DTO.Enums.Orders;
using HQConnector.Dto.DTO.Response;
using HQConnector.Dto.DTO.Response.Error;
using HQConnector.Dto.DTO.Response.Interfaces;
using HQConnector.Dto.DTO.Trade;
using FinancialStatistics.Models;

namespace FinancialStatistics
{
    public static class CalculateDeal
    {
        public static IResultResponse<Deals> CalculateDealsByTillZeroMethod(IEnumerable<MyTrade> trades)
        {
            if (trades != null && trades.All(p=> p != null) && trades.ToList().Count >= 2)
            {
                try
                {
                    var dealList = new List<Deal>();                   
                    var pairs = trades.ToList().Select(p => p.Pair).Distinct().ToList();
                    var restemplist = new List<MyTrade>();
                    foreach (var pair in pairs) 
                    {                      
                        
                        var tradelist = trades.Where(p => p.Pair == pair).OrderBy(p => p.Time.Date).ThenBy(p => p.Time.TimeOfDay).ToList();
                        var tempList = new List<MyTrade>();
                        foreach (var trade in tradelist)
                        {
                            if (tempList.Count() == 0)
                            {
                                tempList.Add(trade);
                                continue;
                            }
                            tempList.Add(trade);

                            // Sum all buy ans sell volumes and compare it

                            if (tempList.Where(tr => tr.Side == Sides.Buy).Sum(p => p.Amount) -
                                tempList.Where(tr => tr.Side == Sides.Sell).Sum(p => p.Amount) == 0)
                            {
                                var tmplistopen = new List<MyTrade>();
                                var tmplistclose = new List<MyTrade>();

                                if (tempList.First().Side == Sides.Buy)
                                {
                                    tmplistopen.AddRange(tempList.Where(tr => tr.Side == Sides.Buy));
                                    tmplistclose.AddRange(tempList.Where(tr => tr.Side == Sides.Sell));
                                }
                                else
                                {
                                    tmplistopen.AddRange(tempList.Where(tr => tr.Side == Sides.Sell));
                                    tmplistclose.AddRange(tempList.Where(tr => tr.Side == Sides.Buy));
                                }
                                decimal openmidleprice = tmplistopen.Aggregate<MyTrade, decimal>(0,
                                    (current, tr) => current + (tr.Price * tr.Amount));
                                openmidleprice = Math.Abs(openmidleprice / tmplistopen.Sum(p => Math.Abs(p.Amount)));
                                decimal closemidleprice = tmplistclose.Aggregate<MyTrade, decimal>(0,
                                    (current, tr) => current + (tr.Price * tr.Amount));
                                closemidleprice = Math.Abs(closemidleprice / tmplistclose.Sum(p => p.Amount));
                                decimal dealvolume = tempList.Sum(p => Math.Abs(p.Amount)) / 2;


                                decimal resinpoints = -((openmidleprice * tmplistopen.Sum(p => p.Amount)) - (closemidleprice * tmplistclose.Sum(p => p.Amount))) / (dealvolume);

                                Sides side = Sides.Buy;

                                if (tempList.First().Side == Sides.Sell)
                                {
                                    side = Sides.Sell;
                                    resinpoints = ((openmidleprice * tmplistopen.Sum(p => p.Amount)) - (closemidleprice * tmplistclose.Sum(p => p.Amount))) / (dealvolume);
                                }

                                decimal currentcomission = 0;
                                if (tempList.All(p => p.Commission != null))
                                {
                                    currentcomission = tempList.Sum(p => p.Commission.CurrentComissionValue);
                                }
                                decimal totalresult = resinpoints * dealvolume;
                                decimal moneyresult = totalresult - currentcomission;
                                dealList.Add(new Deal(tempList.First().Pair, side, tempList.First().Time.DateTime, tempList.LastOrDefault().Time.DateTime, openmidleprice, closemidleprice,
                                    dealvolume, currentcomission, resinpoints, totalresult, moneyresult * trade.BaseCurrencyStepOrCost));

                                tempList.Clear();
                                continue;

                            }
                            // Check trades if we have reverse deal
                            if (tempList.First().Side == Sides.Buy)
                            {
                                if (tempList.Where(tr => tr.Side == Sides.Buy).Sum(p => p.Amount) < tempList.Where(tr => tr.Side == Sides.Sell).Sum(p => p.Amount))
                                {
                                    var tmplistopen = new List<MyTrade>();
                                    var tmplistclose = new List<MyTrade>();
                                    tmplistopen.AddRange(tempList.Where(tr => tr.Side == Sides.Buy));
                                    tmplistclose.AddRange(tempList.Where(tr => tr.Side == Sides.Sell));

                                    decimal dealvolume = tmplistopen.Sum(p => Math.Abs(p.Amount));

                                    Sides side = Sides.Buy;
                                    decimal openmidleprice = tmplistopen.Aggregate<MyTrade, decimal>(0,
                                    (current, tr) => current + (tr.Price * tr.Amount));
                                    openmidleprice = Math.Abs(openmidleprice / tmplistopen.Sum(p => Math.Abs(p.Amount)));

                                    var revesredeal = tmplistclose.Last();

                                    decimal reversetradeamount = tmplistclose.Sum(p => p.Amount) - dealvolume;
                                    CommissionModel _lasttradecommission = revesredeal.Commission;
                                    if (_lasttradecommission != null)
                                    {
                                        _lasttradecommission.CurrentComissionValue = CommissionHelper.CalculateCommissionOnCommissionRate(revesredeal.Commission.CurrentComissionRate, revesredeal.Price, revesredeal.Amount - reversetradeamount, revesredeal.CommissionTakerOrMaker);
                                    }

                                    var lasttradefordeal = new MyTrade
                                    {
                                        Account = revesredeal.Account,
                                        Amount = revesredeal.Amount - reversetradeamount,
                                        BaseCurrencyStepOrCost = revesredeal.BaseCurrencyStepOrCost,
                                        CommissionTakerOrMaker = revesredeal.CommissionTakerOrMaker,
                                        Commission = _lasttradecommission,
                                        Exchange = revesredeal.Exchange,
                                        Id = revesredeal.Id,
                                        OrderId = revesredeal.OrderId,
                                        Pair = revesredeal.Pair,
                                        Price = revesredeal.Price,
                                        Side = revesredeal.Side,
                                        Time = revesredeal.Time

                                    };

                                    tmplistclose.Remove(revesredeal);
                                    tmplistclose.Add(lasttradefordeal);
                                    decimal closemidleprice = tmplistclose.Aggregate<MyTrade, decimal>(0,
                                    (current, tr) => current + (tr.Price * tr.Amount));
                                    closemidleprice = Math.Abs(closemidleprice / tmplistclose.Sum(p => p.Amount));

                                    decimal resinpoints = -((openmidleprice * tmplistopen.Sum(p => p.Amount)) - (closemidleprice * tmplistclose.Sum(p => p.Amount))) / (dealvolume);
                                    decimal currentcomission = 0;
                                    if (tmplistopen.All(p => p.Commission != null) && tmplistclose.All(p => p.Commission != null))
                                    {
                                        currentcomission = tmplistopen.Sum(p => p.Commission.CurrentComissionValue) + tmplistclose.Sum(p => p.Commission.CurrentComissionValue);
                                    }
                                    decimal totalresult = resinpoints * dealvolume;
                                    decimal moneyresult = totalresult - currentcomission;
                                    dealList.Add(new Deal(tempList.First().Pair, side, tempList.First().Time.DateTime, tempList.LastOrDefault().Time.DateTime, openmidleprice, closemidleprice,
                                        dealvolume, currentcomission, resinpoints, totalresult, moneyresult * trade.BaseCurrencyStepOrCost));

                                    tempList.Clear();
                                    CommissionModel _newtradecommission = revesredeal.Commission;
                                    if (_lasttradecommission != null)
                                    {
                                        _newtradecommission.CurrentComissionValue = CommissionHelper.CalculateCommissionOnCommissionRate(revesredeal.Commission.CurrentComissionRate, revesredeal.Price, reversetradeamount, revesredeal.CommissionTakerOrMaker);
                                    }
                                    tempList.Add(new MyTrade()
                                    {
                                        Account = revesredeal.Account,
                                        Amount = reversetradeamount,
                                        BaseCurrencyStepOrCost = revesredeal.BaseCurrencyStepOrCost,
                                        CommissionTakerOrMaker = revesredeal.CommissionTakerOrMaker,
                                        Commission = _newtradecommission,
                                        Exchange = revesredeal.Exchange,
                                        Id = revesredeal.Id,
                                        OrderId = revesredeal.OrderId,
                                        Pair = revesredeal.Pair,
                                        Price = revesredeal.Price,
                                        Side = revesredeal.Side,
                                        Time = revesredeal.Time
                                    });
                                    continue;
                                }
                            }
                            else
                            {
                                if (tempList.Where(tr => tr.Side == Sides.Sell).Sum(p => p.Amount) < tempList.Where(tr => tr.Side == Sides.Buy).Sum(p => p.Amount))
                                {
                                    var tmplistopen = new List<MyTrade>();
                                    var tmplistclose = new List<MyTrade>();
                                    tmplistopen.AddRange(tempList.Where(tr => tr.Side == Sides.Sell));
                                    tmplistclose.AddRange(tempList.Where(tr => tr.Side == Sides.Buy));

                                    decimal dealvolume = tmplistopen.Sum(p => Math.Abs(p.Amount));

                                    Sides side = Sides.Sell;
                                    decimal openmidleprice = tmplistopen.Aggregate<MyTrade, decimal>(0,
                                    (current, tr) => current + (tr.Price * tr.Amount));
                                    openmidleprice = Math.Abs(openmidleprice / tmplistopen.Sum(p => Math.Abs(p.Amount)));

                                    var revesredeal = tmplistclose.Last();

                                    decimal reversetradeamount = tmplistclose.Sum(p => p.Amount) - dealvolume;
                                    var persentagefromlastdeal = (reversetradeamount / revesredeal.Amount);
                                    CommissionModel _lasttradecommission = revesredeal.Commission;
                                    if (_lasttradecommission != null)
                                    {
                                        _lasttradecommission.CurrentComissionValue = CommissionHelper.CalculateCommissionOnCommissionRate(revesredeal.Commission.CurrentComissionRate, revesredeal.Price, revesredeal.Amount - reversetradeamount, revesredeal.CommissionTakerOrMaker);
                                    }
                                    var lasttradefordeal = new MyTrade
                                    {
                                        Account = revesredeal.Account,
                                        Amount = revesredeal.Amount - reversetradeamount,
                                        BaseCurrencyStepOrCost = revesredeal.BaseCurrencyStepOrCost,
                                        CommissionTakerOrMaker = revesredeal.CommissionTakerOrMaker,
                                        Commission = _lasttradecommission,
                                        Exchange = revesredeal.Exchange,
                                        Id = revesredeal.Id,
                                        OrderId = revesredeal.OrderId,
                                        Pair = revesredeal.Pair,
                                        Price = revesredeal.Price,
                                        Side = revesredeal.Side,
                                        Time = revesredeal.Time

                                    };

                                    tmplistclose.Remove(revesredeal);
                                    tmplistclose.Add(lasttradefordeal);
                                    decimal closemidleprice = tmplistclose.Aggregate<MyTrade, decimal>(0,
                                    (current, tr) => current + (tr.Price * tr.Amount));
                                    closemidleprice = Math.Abs(closemidleprice / tmplistclose.Sum(p => p.Amount));

                                    decimal resinpoints = ((openmidleprice * tmplistopen.Sum(p => p.Amount)) - (closemidleprice * tmplistclose.Sum(p => p.Amount))) / (dealvolume);
                                    decimal currentcomission = 0;
                                    if (tmplistopen.All(p => p.Commission != null) && tmplistclose.All(p => p.Commission != null))
                                    {
                                        currentcomission = tmplistopen.Sum(p => p.Commission.CurrentComissionValue) + tmplistclose.Sum(p => p.Commission.CurrentComissionValue);
                                    }
                                    decimal totalresult = resinpoints * dealvolume;
                                    decimal moneyresult = totalresult - currentcomission;
                                    dealList.Add(new Deal(tempList.First().Pair, side, tempList.First().Time.DateTime, tempList.LastOrDefault().Time.DateTime, openmidleprice, closemidleprice,
                                        dealvolume, currentcomission, resinpoints, totalresult, moneyresult * trade.BaseCurrencyStepOrCost));

                                    tempList.Clear();
                                    CommissionModel _newtradecommission = revesredeal.Commission;
                                    if (_lasttradecommission != null)
                                    {
                                        _newtradecommission.CurrentComissionValue = CommissionHelper.CalculateCommissionOnCommissionRate(revesredeal.Commission.CurrentComissionRate, revesredeal.Price, reversetradeamount, revesredeal.CommissionTakerOrMaker);
                                    }
                                    tempList.Add(new MyTrade()
                                    {
                                        Account = revesredeal.Account,
                                        Amount = reversetradeamount,
                                        BaseCurrencyStepOrCost = revesredeal.BaseCurrencyStepOrCost,
                                        CommissionTakerOrMaker = revesredeal.CommissionTakerOrMaker,
                                        Commission = _newtradecommission,
                                        Exchange = revesredeal.Exchange,
                                        Id = revesredeal.Id,
                                        OrderId = revesredeal.OrderId,
                                        Pair = revesredeal.Pair,
                                        Price = revesredeal.Price,
                                        Side = revesredeal.Side,
                                        Time = revesredeal.Time
                                    });
                                    continue;
                                }
                            }

                        }
                        restemplist.AddRange(tempList);
                    }

                    return new MessageResponse<Deals>(new Deals(dealList, restemplist), new SuccessResponse());
                }
                catch (Exception ex) 
                {
                    return new MessageResponse<Deals>(null, new UnknownError(ex.Message.ToString()));

                }
            }

            return new MessageResponse<Deals>(null, new UnknownError("Argument MyTrades is null or not enough MyTrades for calculation"));
        }


        public static IResultResponse<Deals> CalculateDealsByFIFOMethod(IEnumerable<MyTrade> trades)
        {
            if (trades != null && trades.All(p => p != null) && trades.ToList().Count >= 2)
            {
                try
                {
                    var dealList = new List<Deal>();
                    var pairs = trades.ToList().Select(p => p.Pair).Distinct().ToList();
                    var restemplist = new List<MyTrade>();

                    foreach (var pair in pairs)
                    {
                        var tradelist = trades.Where(p => p.Pair == pair).OrderBy(p => p.Time.Date).ThenBy(p => p.Time.TimeOfDay).ToList();
                        var tempList = new List<MyTrade>();

                        foreach (var trade in tradelist)
                        {
                            if (tempList.Count == 0)
                            {
                                tempList.Add(trade);
                                continue;
                            }

                            // Compare trades volume 
                            if (tempList.First().Side == Sides.Buy && trade.Side == Sides.Sell)
                            {
                                // if trade with full amount are equal
                                if (tempList.First().Amount - trade.Amount == 0)
                                {
                                    var side = Sides.Buy;
                                    decimal openprice = tempList.First().Price;
                                    decimal closeprice = trade.Price;
                                    decimal dealvolume = Math.Abs(tempList.First().Amount);

                                    decimal resinpoints = -(openprice - closeprice);
                                    decimal currentcomission = 0;
                                    if (tempList.First().Commission != null && trade.Commission != null)
                                    {
                                        currentcomission = tempList.First().Commission.CurrentComissionValue + trade.Commission.CurrentComissionValue;
                                    }

                                    
                                    decimal totalresult = resinpoints * dealvolume;
                                    decimal moneyresult = totalresult - currentcomission;

                                    dealList.Add(new Deal(tempList.First().Pair, side, tempList.First().Time.DateTime,
                                        trade.Time.DateTime, openprice, closeprice,
                                        dealvolume, currentcomission, resinpoints, totalresult,
                                        moneyresult * trade.BaseCurrencyStepOrCost));
                                    tempList.Remove(tempList.First());
                                    continue;
                                }

                                // if first trade amount more than closing trade, we will safe part amount of first deal
                                if (tempList.First().Amount > trade.Amount)
                                {
                                    var side = Sides.Buy;
                                    decimal openprice = tempList.First().Price;
                                    decimal closeprice = trade.Price;
                                    decimal dealvolume = Math.Abs(trade.Amount);

                                    decimal resinpoints = -(openprice - closeprice);
                                    CommissionModel _firsttradecommission = tempList.First().Commission;
                                    if (_firsttradecommission != null)
                                    {
                                        _firsttradecommission.CurrentComissionValue = CommissionHelper.CalculateCommissionOnCommissionRate(tempList.First().Commission.CurrentComissionRate, tempList.First().Price,
                                            dealvolume, tempList.First().CommissionTakerOrMaker);
                                    }
                                    decimal currentcomission = 0;
                                    if (_firsttradecommission != null && trade.Commission != null)
                                    {
                                        currentcomission = _firsttradecommission.CurrentComissionValue + trade.Commission.CurrentComissionValue;
                                    }
                                    decimal totalresult = resinpoints * dealvolume;
                                    decimal moneyresult = totalresult - currentcomission;

                                    dealList.Add(new Deal(tempList.First().Pair, side, tempList.First().Time.DateTime,
                                        trade.Time.DateTime, openprice, closeprice,
                                        dealvolume, currentcomission, resinpoints, totalresult,
                                        moneyresult * trade.BaseCurrencyStepOrCost));

                                    tempList.First().Amount = tempList.First().Amount - trade.Amount;

                                    if (tempList.First().Commission != null)
                                    {
                                        tempList.First().Commission.CurrentComissionValue =
                                            CommissionHelper.CalculateCommissionOnCommissionRate(tempList.First().Commission.CurrentComissionRate, tempList.First().Price,
                                                tempList.First().Amount, tempList.First().CommissionTakerOrMaker);
                                    }

                                    continue;

                                }

                                // if direction of deal will change with new volume
                                if (tempList.First().Amount < trade.Amount)
                                {
                                    var curtrade = trade;

                                    for (int i = 0; i < tempList.Count(); i++)
                                    {
                                        if (tempList[i].Amount < curtrade.Amount)
                                        {
                                            var side = Sides.Buy;
                                            decimal openprice = tempList[i].Price;
                                            decimal closeprice = curtrade.Price;
                                            decimal dealvolume = Math.Abs(tempList[i].Amount);

                                            decimal resinpoints = -(openprice - closeprice);
                                            CommissionModel _lasttradecommission = curtrade.Commission;
                                            if (_lasttradecommission != null)
                                            {
                                                _lasttradecommission.CurrentComissionValue = CommissionHelper.CalculateCommissionOnCommissionRate(curtrade.Commission.CurrentComissionRate, curtrade.Price,
                                                    dealvolume, curtrade.CommissionTakerOrMaker);
                                            }
                                            decimal currentcomission = 0;
                                            if (_lasttradecommission != null && tempList[i].Commission != null)
                                            {
                                                currentcomission = tempList[i].Commission.CurrentComissionValue + _lasttradecommission.CurrentComissionValue;
                                            }
                                        
                                            decimal totalresult = resinpoints * dealvolume;
                                            decimal moneyresult = totalresult - currentcomission;

                                            dealList.Add(new Deal(tempList[i].Pair, side, tempList[i].Time.DateTime,
                                                curtrade.Time.DateTime, openprice, closeprice,
                                                dealvolume, currentcomission, resinpoints, totalresult,
                                                moneyresult * curtrade.BaseCurrencyStepOrCost));

                                            curtrade.Amount = curtrade.Amount - tempList[i].Amount;

                                            if (curtrade.Commission != null)
                                            {
                                                curtrade.Commission.CurrentComissionValue =
                                                    CommissionHelper.CalculateCommissionOnCommissionRate(curtrade.Commission.CurrentComissionRate, curtrade.Price,
                                                        curtrade.Amount, curtrade.CommissionTakerOrMaker);
                                            }
                                        

                                            tempList.Remove(tempList[i]);
                                            i--;
                                            if (tempList.Count() == 0)
                                            {
                                                tempList.Add(curtrade);
                                                break;
                                            }

                                            continue;
                                        }

                                        if (tempList[i].Amount == curtrade.Amount)
                                        {
                                            var side = Sides.Buy;
                                            decimal openprice = tempList[i].Price;
                                            decimal closeprice = curtrade.Price;
                                            decimal dealvolume = Math.Abs(tempList[i].Amount);

                                            decimal resinpoints = -(openprice - closeprice);
                                           
                                            decimal currentcomission = 0;
                                            if (tempList[i].Commission != null && curtrade.Commission != null)
                                            {
                                                currentcomission = tempList[i].Commission.CurrentComissionValue +
                                                                   curtrade.Commission.CurrentComissionValue;
                                            }

                                            decimal totalresult = resinpoints * dealvolume;
                                            decimal moneyresult = totalresult - currentcomission;

                                            dealList.Add(new Deal(tempList[i].Pair, side, tempList[i].Time.DateTime,
                                                curtrade.Time.DateTime, openprice, closeprice,
                                                dealvolume, currentcomission, resinpoints, totalresult,
                                                moneyresult * curtrade.BaseCurrencyStepOrCost));
                                            tempList.Remove(tempList[i]);
                                            break;
                                        }

                                        if (tempList[i].Amount > curtrade.Amount)
                                        {
                                            var side = Sides.Buy;
                                            decimal openprice = tempList[i].Price;
                                            decimal closeprice = curtrade.Price;
                                            decimal dealvolume = Math.Abs(curtrade.Amount);

                                            decimal resinpoints = -(openprice - closeprice);
                                            decimal currentcomission = 0;
                                            CommissionModel _firsttradecommission = tempList[i].Commission;
                                            if (_firsttradecommission != null)
                                            {
                                                _firsttradecommission.CurrentComissionValue = CommissionHelper.CalculateCommissionOnCommissionRate(tempList[i].Commission.CurrentComissionRate, tempList[i].Price,
                                                    dealvolume, tempList[i].CommissionTakerOrMaker);
                                            }
                                           
                                            if (_firsttradecommission != null && trade.Commission != null)
                                            {
                                                currentcomission = _firsttradecommission.CurrentComissionValue + trade.Commission.CurrentComissionValue;
                                            }
                                           
                                            decimal totalresult = resinpoints * dealvolume;
                                            decimal moneyresult = totalresult - currentcomission;

                                            dealList.Add(new Deal(tempList[i].Pair, side, tempList[i].Time.DateTime,
                                                curtrade.Time.DateTime, openprice, closeprice,
                                                dealvolume, currentcomission, resinpoints, totalresult,
                                                moneyresult * curtrade.BaseCurrencyStepOrCost));

                                            tempList[i].Amount = tempList[i].Amount - curtrade.Amount;

                                            if (tempList[i].Commission != null)
                                            {
                                                tempList[i].Commission.CurrentComissionValue =
                                                    CommissionHelper.CalculateCommissionOnCommissionRate(tempList[i].Commission.CurrentComissionRate, tempList[i].Price,
                                                        tempList[i].Amount, tempList[i].CommissionTakerOrMaker);
                                            }
                                            break;
                                        }
                                    }

                                 
                                }

                            }
                            else if (tempList.First().Side == Sides.Buy && trade.Side == Sides.Buy)
                            {
                                tempList.Add(trade);
                                continue;
                            }
                            else if (tempList.First().Side == Sides.Sell && trade.Side == Sides.Buy)
                            {
                                // if trade with full amount are equal
                                if (tempList.First().Amount - trade.Amount == 0)
                                {
                                    var side = Sides.Sell;
                                    decimal openprice = tempList.First().Price;
                                    decimal closeprice = trade.Price;
                                    decimal dealvolume = Math.Abs(tempList.First().Amount);

                                    decimal resinpoints = openprice - closeprice;
                                    decimal currentcomission = 0;
                                    if (tempList.First().Commission != null && trade.Commission != null)
                                    {
                                        currentcomission = tempList.First().Commission.CurrentComissionValue + trade.Commission.CurrentComissionValue;
                                    }
                                    decimal totalresult = resinpoints * dealvolume;
                                    decimal moneyresult = totalresult - currentcomission;

                                    dealList.Add(new Deal(tempList.First().Pair, side, tempList.First().Time.DateTime, trade.Time.DateTime, openprice, closeprice,
                                        dealvolume, currentcomission, resinpoints, totalresult, moneyresult * trade.BaseCurrencyStepOrCost));
                                    tempList.Remove(tempList.First());
                                    continue;
                                }

                                // if first trade amount more than closing trade, we will safe part amount of first deal
                                if (tempList.First().Amount > trade.Amount)
                                {
                                    var side = Sides.Sell;
                                    decimal openprice = tempList.First().Price;
                                    decimal closeprice = trade.Price;
                                    decimal dealvolume = Math.Abs(trade.Amount);

                                    decimal resinpoints = openprice - closeprice;
                                    CommissionModel _firsttradecommission = tempList.First().Commission;
                                    if (_firsttradecommission != null)
                                    {
                                        _firsttradecommission.CurrentComissionValue = CommissionHelper.CalculateCommissionOnCommissionRate(tempList.First().Commission.CurrentComissionRate, tempList.First().Price,
                                            dealvolume, tempList.First().CommissionTakerOrMaker);
                                    }
                                    decimal currentcomission = 0;
                                    if (_firsttradecommission != null && trade.Commission != null)
                                    {
                                        currentcomission = _firsttradecommission.CurrentComissionValue + trade.Commission.CurrentComissionValue;
                                    }
                                    
                                    decimal totalresult = resinpoints * dealvolume;
                                    decimal moneyresult = totalresult - currentcomission;

                                    dealList.Add(new Deal(tempList.First().Pair, side, tempList.First().Time.DateTime, trade.Time.DateTime, openprice, closeprice,
                                        dealvolume, currentcomission, resinpoints, totalresult, moneyresult * trade.BaseCurrencyStepOrCost));

                                    tempList.First().Amount = tempList.First().Amount - trade.Amount;

                                    if (tempList.First().Commission != null)
                                    {
                                        tempList.First().Commission.CurrentComissionValue =
                                            CommissionHelper.CalculateCommissionOnCommissionRate(tempList.First().Commission.CurrentComissionRate, tempList.First().Price,
                                                tempList.First().Amount, tempList.First().CommissionTakerOrMaker);
                                    }
                                    continue;
                                }

                                // if direction of deal will change with new volume
                                if (tempList.First().Amount < trade.Amount)
                                {
                                    var curtrade = trade;
                                    for (int i = 0; i < tempList.Count(); i++)
                                    {
                                        if (tempList[i].Amount < curtrade.Amount)
                                        {
                                            var side = Sides.Sell;
                                            decimal openprice = tempList[i].Price;
                                            decimal closeprice = curtrade.Price;
                                            decimal dealvolume = Math.Abs(tempList[i].Amount);

                                            decimal resinpoints = openprice - closeprice;
                                            CommissionModel _lasttradecommission = curtrade.Commission;
                                            if (_lasttradecommission != null)
                                            {
                                                _lasttradecommission.CurrentComissionValue = CommissionHelper.CalculateCommissionOnCommissionRate(curtrade.Commission.CurrentComissionRate, curtrade.Price,
                                                    dealvolume, curtrade.CommissionTakerOrMaker);
                                            }
                                            decimal currentcomission = 0;
                                            if (_lasttradecommission != null && tempList[i].Commission != null)
                                            {
                                                currentcomission = tempList[i].Commission.CurrentComissionValue + _lasttradecommission.CurrentComissionValue;
                                            }
                                           
                                            decimal totalresult = resinpoints * dealvolume;
                                            decimal moneyresult = totalresult - currentcomission;

                                            dealList.Add(new Deal(tempList[i].Pair, side, tempList[i].Time.DateTime, curtrade.Time.DateTime, openprice, closeprice,
                                                dealvolume, currentcomission, resinpoints, totalresult, moneyresult * curtrade.BaseCurrencyStepOrCost));
                                            curtrade.Amount = curtrade.Amount - tempList[i].Amount;

                                            if (curtrade.Commission != null)
                                            {
                                                curtrade.Commission.CurrentComissionValue =
                                                    CommissionHelper.CalculateCommissionOnCommissionRate(curtrade.Commission.CurrentComissionRate, curtrade.Price,
                                                        curtrade.Amount, curtrade.CommissionTakerOrMaker);
                                            }
                                            
                                            tempList.Remove(tempList[i]);
                                            i--;
                                            if (tempList.Count() == 0)
                                            {
                                                tempList.Add(curtrade);
                                                break;
                                            }
                                            continue;
                                        }
                                        if (tempList[i].Amount == curtrade.Amount)
                                        {
                                            var side = Sides.Sell;
                                            decimal openprice = tempList[i].Price;
                                            decimal closeprice = curtrade.Price;
                                            decimal dealvolume = Math.Abs(tempList[i].Amount);

                                            decimal resinpoints = (openprice - closeprice);
                                            decimal currentcomission = 0;
                                            if (tempList[i].Commission != null && curtrade.Commission != null)
                                            {
                                                currentcomission = tempList[i].Commission.CurrentComissionValue +
                                                                   curtrade.Commission.CurrentComissionValue;
                                            }
                                           
                                            decimal totalresult = resinpoints * dealvolume;
                                            decimal moneyresult = totalresult - currentcomission;

                                            dealList.Add(new Deal(tempList[i].Pair, side, tempList[i].Time.DateTime, curtrade.Time.DateTime, openprice, closeprice,
                                                dealvolume, currentcomission, resinpoints, totalresult, moneyresult * curtrade.BaseCurrencyStepOrCost));
                                            tempList.Remove(tempList[i]);
                                            break;
                                        }
                                        if (tempList[i].Amount > curtrade.Amount)
                                        {
                                            var side = Sides.Sell;
                                            decimal openprice = tempList[i].Price;
                                            decimal closeprice = curtrade.Price;
                                            decimal dealvolume = Math.Abs(curtrade.Amount);

                                            decimal resinpoints = (openprice - closeprice);
                                            decimal currentcomission = 0;
                                            CommissionModel _firsttradecommission = tempList[i].Commission;
                                            if (_firsttradecommission != null)
                                            {
                                                _firsttradecommission.CurrentComissionValue = CommissionHelper.CalculateCommissionOnCommissionRate(tempList[i].Commission.CurrentComissionRate, tempList[i].Price,
                                                    dealvolume, tempList[i].CommissionTakerOrMaker);
                                            }

                                            if (_firsttradecommission != null && trade.Commission != null)
                                            {
                                                currentcomission = _firsttradecommission.CurrentComissionValue + trade.Commission.CurrentComissionValue;
                                            }
                                         
                                            decimal totalresult = resinpoints * dealvolume;
                                            decimal moneyresult = totalresult - currentcomission;

                                            dealList.Add(new Deal(tempList[i].Pair, side, tempList[i].Time.DateTime, curtrade.Time.DateTime, openprice, closeprice,
                                                dealvolume, currentcomission, resinpoints, totalresult, moneyresult * curtrade.BaseCurrencyStepOrCost));

                                            tempList[i].Amount = tempList[i].Amount - curtrade.Amount;

                                            if (tempList[i].Commission != null)
                                            {
                                                tempList[i].Commission.CurrentComissionValue =
                                                    CommissionHelper.CalculateCommissionOnCommissionRate(tempList[i].Commission.CurrentComissionRate, tempList[i].Price,
                                                        tempList[i].Amount, tempList[i].CommissionTakerOrMaker);
                                            }
                                            break;
                                        }
                                    }
                                   
                                }
                            }
                            else if (tempList.First().Side == Sides.Sell && trade.Side == Sides.Sell)
                            {
                                tempList.Add(trade);
                        
                            }
                        }
                        restemplist.AddRange(tempList);
                    }
                  

                    return new MessageResponse<Deals>(new Deals(dealList, restemplist), new SuccessResponse());
                }
                catch (Exception ex)
                {
                    return new MessageResponse<Deals>(null, new UnknownError(ex.Message.ToString()));

                }
            }

            return new MessageResponse<Deals>(null, new UnknownError("Argument MyTrades is null or not enough MyTrades for calculation"));
        }
    }
}
