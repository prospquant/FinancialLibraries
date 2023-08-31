using HQConnector.Dto.DTO.Trade;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialStatistics.Models
{
    public class Deals
    {
        public IEnumerable<Deal> FormedDeals { get; }

        public IEnumerable<MyTrade> RemainingTrades { get; }

        public Deals(IEnumerable<Deal> formedDeals, IEnumerable<MyTrade> remainingTrades)  
        {
            FormedDeals = formedDeals;
            RemainingTrades = remainingTrades;
        }
    }
}
