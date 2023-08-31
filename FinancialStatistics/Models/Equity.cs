using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialStatistics.Models
{
    public class Equity
    {
        public Deals CurrentDeals { get; }

        public IEnumerable<decimal> EquityCurve { get; }

        public Equity(Deals currentDeals, IEnumerable<decimal> equityCurve) 
        {
            CurrentDeals = currentDeals;
            EquityCurve = equityCurve;
        }
    }
}
