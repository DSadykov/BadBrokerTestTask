using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

using BadBrokerTestTask.Models;
using BadBrokerTestTask.Models.Responses;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace BadBrokerTestTask.Services.Tests
{
    public class RevenueCalculatorTests
    {

        [Fact()]
        public void CalculateHighestRevenueTest()
        {
            //Arrange

            var appSettings = @"{
  ""ApiKey"": ""d35f20fe42aa46c28db5bb0b5df7c1bf"",
  ""RequiredCurrencies"": ""rub,eur,gbp,jpy""
}
";

            ConfigurationBuilder builder = new();

            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            IConfigurationRoot configuration = builder.Build();

            Mock<ILogger<RevenueCalculator>> logger = new();

            RevenueCalculator revenueCalculator = new(logger.Object, configuration);

            List<CurrencyRateModel> currencies = new() { new CurrencyRateModel()
                {
                    DateTime = new(2022, 11, 13),
                    Rates=new Dictionary<string, decimal>()
                    {
                        {"eur",2},
                        {"gbp",3},
                        {"rub",4},
                        {"jpy",5}
                    }
                } ,new CurrencyRateModel()
                {
                    DateTime = new(2022, 11, 14),
                    Rates=new Dictionary<string, decimal>()
                    {
                        {"eur",1},
                        {"gbp",3},
                        {"rub",4},
                        {"jpy",5}
                    }
                }
            };

            //Act

            BestRatesResponse result = revenueCalculator.CalculateHighestRevenue(currencies, 100);

            //Assert
            Assert.Equal(JsonSerializer.Serialize(new List<Rate>()
            {
                new()
                {
                    Date=new(2022, 11, 13),
                    Eur=2,
                    Gbp=3,
                    Rub=4,
                    Jpy=5
                },
                new()
                {
                    Date=new(2022, 11, 14),
                    Eur=1,
                    Gbp=3,
                    Rub=4,
                    Jpy=5
                }
            }), JsonSerializer.Serialize(result.Rates));
            Assert.Equal(98, result.Revenue);
            Assert.Equal(new(2022, 11, 14), result.SellDate);
            Assert.Equal(new(2022, 11, 13), result.Buydate);
            Assert.Equal("eur", result.Tool);

        }
    }
}