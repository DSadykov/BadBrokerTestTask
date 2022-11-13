using Xunit;
using BadBrokerTestTask.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using BadBrokerTestTask.Models;

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

            var builder = new ConfigurationBuilder();

            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            var configuration = builder.Build();

            var logger = new Mock<ILogger<RevenueCalculator>>();

            var revenueCalculator = new RevenueCalculator(logger.Object, configuration);

            var currencies = new List<CurrencyRateModel>() { new CurrencyRateModel()
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

            var result = revenueCalculator.CalculateHighestRevenue(currencies, 100);

            //Assert

            Assert.Equal(98, result.Revenue);
            Assert.Equal(new(2022, 11, 14), result.SellDate);
            Assert.Equal(new(2022, 11, 13), result.Buydate);
            Assert.Equal("eur", result.Tool);
            
        }
    }
}