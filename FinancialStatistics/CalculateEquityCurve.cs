using HQConnector.Dto.DTO.Response;
using HQConnector.Dto.DTO.Response.Error;
using HQConnector.Dto.DTO.Response.Interfaces;
using FinancialStatistics.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace FinancialStatistics
{
    public static class CalculateEquityCurve
    {
        public static IResultResponse<Equity> CalculateEquity(Deals currentDeals,decimal balance) 
        {
            var equityCurve = new List<decimal>();
            decimal currezult = 0;
            if (currentDeals != null && currentDeals.FormedDeals != null && currentDeals.FormedDeals.Count() != 0 && balance != 0)
            {
                try
                {
                    foreach (var deal in currentDeals.FormedDeals)
                    {
                        currezult += ((deal.MoneyRes / balance) * 100);
                        equityCurve.Add(currezult);
                    }
                }
                catch (Exception ex) 
                {
                    return new MessageResponse<Equity>(null, new UnknownError(ex.Message.ToString()));
                }
            }

            return new MessageResponse<Equity>(new Equity(currentDeals,equityCurve), new SuccessResponse());

        }
    }
}
