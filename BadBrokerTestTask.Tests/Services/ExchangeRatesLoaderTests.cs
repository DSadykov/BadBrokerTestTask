using Xunit;
using BadBrokerTestTask.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BadBrokerTestTask.Services.Tests
{
    public class ExchangeRatesLoaderTests
    {

        [Fact()]
        public async void GetCurrencyRatesTest_ShouldBeOk()
        {
            //Arrange
            var logger1 = new Mock<ILogger<OpenExchangeRatesClient>>();
            DateTime date = new(2022, 11, 13);
            var openExchangeRatesClient = new Mock<IOpenExchangeRatesClient>();
            openExchangeRatesClient.Setup(x => x.GetRatesForADate(date)).ReturnsAsync(new CurrencyRateModel()
            {
                DateTime = date,
                Rates = new Dictionary<string, decimal>()
            {
                {"eur",(decimal)0.95 },
                {"rub",60 }
            }
            });



            var logger = new Mock<ILogger<ExchangeRatesLoader>>().Object;

            var exchangeRatesLoader = new ExchangeRatesLoader(logger, openExchangeRatesClient.Object);

            //Act

            var result = await exchangeRatesLoader.GetCurrencyRates(date, date);

            //Assert

            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.True(result[0].Rates.Count == 2);
            Assert.True(result[0].DateTime == date);
        }
    }
}