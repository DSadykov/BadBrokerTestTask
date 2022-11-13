using System;
using System.Collections.Generic;
using System.Linq;

using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Models;
using BadBrokerTestTask.Models.Responses;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BadBrokerTestTask.Services
{
    public class RevenueCalculator : IRevenueCalculator
    {
        private readonly ILogger<RevenueCalculator> _logger;

        public RevenueCalculator(ILogger<RevenueCalculator> logger)
        {
            _logger = logger;
        }
        public BestRatesResponse CalculateHighestRevenue(List<CurrencyRateModel> currencies, decimal usd)
        {
            var groupedCurrencies = currencies.OrderBy(x => x.DateTime).GroupBy(x => x.Currency);
            var result = new BestRatesResponse();
            foreach (var currenciesItem in groupedCurrencies)
            {
                var counter = 1;
                foreach (var buyCurrency in currenciesItem)
                {
                    foreach (var sellCurrency in currenciesItem.Skip(counter))
                    {
                        var tmpRevenue = CountRevenue(buyCurrency, sellCurrency, usd);
                        if (tmpRevenue > result.Revenue)
                        {
                            result.Revenue = tmpRevenue;
                            result.Buydate = buyCurrency.DateTime;
                            result.SellDate = sellCurrency.DateTime;
                            result.Tool = buyCurrency.Currency;
                        }
                    }
                    counter++;
                }
            }
            return result;
        }

        private decimal CountRevenue(CurrencyRateModel buyCurrency, CurrencyRateModel sellCurrency, decimal usd)
        {
            var daysNumber = 0;
            for (var dt = buyCurrency.DateTime; dt <= sellCurrency.DateTime; dt = dt.AddDays(1))
            {
                daysNumber++;
            }
            return usd - (buyCurrency.Price * usd / sellCurrency.Price) - daysNumber;
        }
    }
}
