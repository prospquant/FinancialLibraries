using System;
using System.Collections.Generic;
using System.Text;
using HQConnector.Dto.DTO.Enums.Orders;

namespace FinancialStatistics.Models
{
    /// <summary>
    /// Operation result from exchange trades
    /// </summary>
    public class Deal
    {
        /// <summary>
        /// Security pair
        /// </summary>
        public string SecurityPair { get; set; }

        /// <summary>
        /// Operation direction
        /// </summary>
        public Sides Side { get; set; }
        
        /// <summary>
        /// Open operation time
        /// </summary>
        public DateTime OpenTime { get; set; }
       
        /// <summary>
        /// Close operation time
        /// </summary>
        public DateTime CloseTime { get; set; }
       
        /// <summary>
        /// Open operation price
        /// </summary>
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// Close operation price
        /// </summary>
        public decimal ClosePrice { get; set; }
      
        /// <summary>
        /// Operation volume
        /// </summary>
        public decimal Volume { get; set; }
      
        /// <summary>
        /// Commission from operation
        /// </summary>
        public decimal Comission { get; set; }
       
        /// <summary>
        /// Result in points
        /// </summary>
        public decimal Resinpoint { get; set; }

        /// <summary>
        /// Summary result with commission
        /// </summary>
        public decimal TotalResult { get; set; }
      
        /// <summary>
        /// Result in base currency
        /// </summary>
        public decimal MoneyRes { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="securityPair">Security pair</param>
        /// <param name="side">Operation direction</param>
        /// <param name="opentime">Open operation time</param>
        /// <param name="closetime"> Close operation time</param>
        /// <param name="openprice">Open operation price</param>
        /// <param name="closeprice">Close operation price</param>
        /// <param name="volume">Operation volume</param>
        /// <param name="comission">Commission from operation</param>
        /// <param name="resinpoint">Result in points</param>
        /// <param name="totalResult">Summary result with commission</param>
        /// <param name="moneyres">Result in base currency</param>
        public Deal(string securityPair, Sides side, DateTime opentime, DateTime closetime,
            decimal openprice, decimal closeprice, decimal volume, decimal comission,
            decimal resinpoint, decimal totalResult, decimal moneyres)
        {
            SecurityPair = securityPair;
            Side = side;
            OpenTime = opentime;
            CloseTime = closetime;
            OpenPrice = openprice;
            ClosePrice = closeprice;
            Volume = volume;
            Comission = comission;
            Resinpoint = resinpoint;
            TotalResult = totalResult;
            MoneyRes = moneyres;
        }

     
    }
}
