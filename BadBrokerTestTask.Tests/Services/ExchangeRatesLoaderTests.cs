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

            var appSettings = @"{
  ""ApiKey"": ""d35f20fe42aa46c28db5bb0b5df7c1bf"",
  ""RequiredCurrencies"": ""rub,eur,gbp,jpy""
}
";

            var builder = new ConfigurationBuilder();

            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            var configuration = builder.Build();
            var logger1 = new Mock<ILogger<OpenExchangeRatesClient>>();
            OpenExchangeRatesClient openExchangeRatesClient = new(configuration, logger1.Object);

            DateTime date = new(2022, 11, 13);

            var logger2 = new Mock<ILogger<ExchangeRatesLoader>>().Object;

            var exchangeRatesLoader = new ExchangeRatesLoader(logger2, openExchangeRatesClient);

            var _requierdCurrencies = configuration["RequiredCurrencies"].Split(',').Select(x => x.ToLower()).ToList();

            //Act

            var result = await exchangeRatesLoader.GetCurrencyRates(date, date);

            //Assert

            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.True(result[0].Rates.Count == 4);
            Assert.All(result[0].Rates.Keys, x => _requierdCurrencies.Contains(x));
            Assert.True(result[0].DateTime == date);
        }
    }
}