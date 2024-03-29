﻿using System;
using System.Collections.Generic;
using System.Linq;

using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Models;
using BadBrokerTestTask.Models.Responses;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BadBrokerTestTask.Services
{
    public class RevenueCalculator : IRevenueCalculator
    {
        private readonly ILogger<RevenueCalculator> _logger;
        private readonly List<string> _requierdCurrencies;

        public RevenueCalculator(ILogger<RevenueCalculator> logger, IConfiguration configuration)
        {
            _logger = logger;
            _requierdCurrencies = configuration["RequiredCurrencies"].Split(',').Select(x => x.ToLower()).ToList();
        }
        /// <summary>
        /// Calculates highest revenue, buy time and sell time and returns them as part of <c>BestRatesResponse</c>
        /// 
        /// </summary>
        /// <returns> Can return negative revenue which means there are no profitable currency
        /// </returns>

        public BestRatesResponse CalculateHighestRevenue(List<CurrencyRateModel> currencies, decimal usd)
        {
            BestRatesResponse result = new()
            {
                Rates = currencies.Select(x => x.ToRate()).ToList(),
                Revenue = decimal.MinValue

            };
            currencies.Sort((x, y) => x.DateTime.CompareTo(y.DateTime));
            //revenue is calculated for each currency specified in the appsettings
            foreach (var currency in _requierdCurrencies)
            {
                var counter = 1;
                foreach (CurrencyRateModel buyCurrency in currencies)
                {
                    foreach (CurrencyRateModel sellCurrency in currencies.Skip(counter))
                    {
                        var tmpRevenue = CountRevenue(buyCurrency, sellCurrency, currency, usd);
                        if (tmpRevenue > result.Revenue)
                        {
                            result.Revenue = tmpRevenue;
                            result.Buydate = buyCurrency.DateTime;
                            result.SellDate = sellCurrency.DateTime;
                            result.Tool = currency;
                        }
                    }
                    counter++;
                }
            }
            _logger.LogInformation($"Successfully calculated highest revenue, revenue is {result.Revenue}, buy at {result.Buydate}, sell at {result.SellDate}, tool is {result.Tool}");
            return result;
        }

        private decimal CountRevenue(CurrencyRateModel buyCurrency, CurrencyRateModel sellCurrency, string currency, decimal usd)
        {
            var daysNumber = 0;
            for (DateTime dt = buyCurrency.DateTime; dt <= sellCurrency.DateTime; dt = dt.AddDays(1))
            {
                daysNumber++;
            }

            if (buyCurrency.Rates.TryGetValue(currency, out var buyRate) && sellCurrency.Rates.TryGetValue(currency, out var sellRate))
            {

                var revenue = (buyRate * usd / sellRate) - usd - daysNumber;
                _logger.LogDebug($"Revenue for {currency} at {buyCurrency.DateTime} is {revenue}");
                return revenue;
            }
            _logger.LogWarning($"{currency} was not found for {buyCurrency.DateTime} date!");
            return usd - daysNumber;
        }
    }
}
