using System;
using System.Collections.Generic;

using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Models;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace BadBrokerTestTask.Services.Tests
{
    public class ExchangeRatesLoaderTests
    {

        [Fact()]
        public async void GetCurrencyRatesTest_ShouldBeOk()
        {
            //Arrange
            Mock<ILogger<OpenExchangeRatesClient>> logger1 = new();
            DateTime date = new(2022, 11, 13);
            Mock<IOpenExchangeRatesClient> openExchangeRatesClient = new();
            openExchangeRatesClient.Setup(x => x.GetRatesForADateAsync(date)).ReturnsAsync(new CurrencyRateModel()
            {
                DateTime = date,
                Rates = new Dictionary<string, decimal>()
            {
                {"eur",(decimal)0.95 },
                {"rub",60 }
            }
            });



            ILogger<ExchangeRatesLoader> logger = new Mock<ILogger<ExchangeRatesLoader>>().Object;

            ExchangeRatesLoader exchangeRatesLoader = new(logger, openExchangeRatesClient.Object);

            //Act

            List<CurrencyRateModel> result = await exchangeRatesLoader.GetCurrencyRatesAsync(date, date);

            //Assert

            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.True(result[0].Rates.Count == 2);
            Assert.True(result[0].DateTime == date);
        }
    }
}