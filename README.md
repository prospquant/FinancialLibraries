# FinancialLibraries
Calculate exchange trades and equity curves and tests

This module offers functionality to calculate trades based on the First In, First Out (FIFO) method and etc, using trading data.

Overview
The function CalculateDeals is designed to process a collection of trades and return a set of calculated deals by following the FIFO methodology or TillZero methodology.

Inputs
IEnumerable<MyTrade> trades: A collection of trade records.
Output
IResultResponse<Deals>: A result response containing the calculated deals.
