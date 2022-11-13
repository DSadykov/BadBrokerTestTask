using Xunit;
using BadBrokerTestTask.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BadBrokerTestTask.Services.Tests
{
    public class OpenExchangeRatesClientTests
    {

        [Fact()]
        public async void GetRatesForADateTest_ShouldBeOk()
        {

            // Arrange


            var logger = new Mock<ILogger<OpenExchangeRatesClient>>();
            var appSettings = @"{
  ""ApiKey"": ""d35f20fe42aa46c28db5bb0b5df7c1bf"",
  ""RequiredCurrencies"": ""rub,eur,gbp,jpy""
}
";

            var builder = new ConfigurationBuilder();

            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            var configuration = builder.Build();

            OpenExchangeRatesClient openExchangeRatesClient = new(configuration, logger.Object);

            DateTime date = new(2022, 11, 13);

            var _requierdCurrencies = configuration["RequiredCurrencies"].Split(',').Select(x => x.ToLower()).ToList();

            // Act

            var result = await openExchangeRatesClient.GetRatesForADate(date);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(4, result.Rates.Count);
            Assert.Equal(date, result.DateTime);
            Assert.All(result.Rates.Keys, x => _requierdCurrencies.Contains(x));
        }
        [Fact()]
        public async void GetRatesForADateTest_ShouldReturnNull()
        {

            // Arrange


            var logger = new Mock<ILogger<OpenExchangeRatesClient>>();
            var appSettings = @"{
  ""ApiKey"": ""d35f20fe42aa46c28db5bb0b5df7c1bf"",
  ""RequiredCurrencies"": ""rub,eur,gbp,jpy""
}
";

            var builder = new ConfigurationBuilder();

            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            var configuration = builder.Build();

            OpenExchangeRatesClient openExchangeRatesClient = new(configuration, logger.Object);

            DateTime date = new(2023, 11, 13);

            // Act

            var result = await openExchangeRatesClient.GetRatesForADate(date);

            // Assert

            Assert.Null(result);
        }
    }
}