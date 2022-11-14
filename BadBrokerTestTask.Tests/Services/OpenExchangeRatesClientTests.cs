using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace BadBrokerTestTask.Services.Tests
{
    public class OpenExchangeRatesClientTests
    {

        [Fact()]
        public async void GetRatesForADateTest_ShouldBeOk()
        {

            // Arrange


            Mock<ILogger<OpenExchangeRatesClient>> logger = new();
            var appSettings = @"{
  ""ApiKey"": ""d35f20fe42aa46c28db5bb0b5df7c1bf"",
  ""RequiredCurrencies"": ""rub,eur,gbp,jpy""
}
";

            ConfigurationBuilder builder = new();

            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            IConfigurationRoot configuration = builder.Build();

            OpenExchangeRatesClient openExchangeRatesClient = new(configuration, logger.Object);

            DateTime date = new(2022, 11, 13);

            List<string> _requierdCurrencies = configuration["RequiredCurrencies"].Split(',').Select(x => x.ToLower()).ToList();

            // Act

            Models.CurrencyRateModel result = await openExchangeRatesClient.GetRatesForADate(date);

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


            Mock<ILogger<OpenExchangeRatesClient>> logger = new();
            var appSettings = @"{
  ""ApiKey"": ""d35f20fe42aa46c28db5bb0b5df7c1bf"",
  ""RequiredCurrencies"": ""rub,eur,gbp,jpy""
}
";

            ConfigurationBuilder builder = new();

            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            IConfigurationRoot configuration = builder.Build();

            OpenExchangeRatesClient openExchangeRatesClient = new(configuration, logger.Object);

            DateTime date = new(2023, 11, 13);

            // Act

            Models.CurrencyRateModel result = await openExchangeRatesClient.GetRatesForADate(date);

            // Assert

            Assert.Null(result);
        }
    }
}