using System;
using System.Collections.Generic;
using System.Linq;

using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Models;

using Microsoft.Extensions.Logging;

using Moq;

using Newtonsoft.Json;

using Xunit;

namespace BadBrokerTestTask.Services.Tests
{
    public class ExchangeRatesLoaderWithCachingTests
    {


        public ExchangeRatesLoaderWithCachingTests()
        {

        }

        [Fact()]
        public async void GetCurrencyRatesTest_AllRatesAreFromCache_ShouldReturnOk()
        {
            //arrange
            DateTime _from = new(2022, 11, 11);
            DateTime _to = new(2022, 11, 12);
            List<CurrencyRateModel> cachedRates = new()
            {
                new CurrencyRateModel(){DateTime=new(2022, 11, 11), Rates=new(){ {"eur",(decimal)1.05 } } },
                new CurrencyRateModel(){DateTime=new(2022, 11, 12), Rates=new(){ {"eur",(decimal)0.9 } } },
            };
            List<CurrencyRateModel> apiRates = new();


            var dbRepositoryMock = new Mock<IDbRepository>();
            dbRepositoryMock.Setup(x => x.GetRates(_from, _to)).ReturnsAsync(cachedRates);
            var _dbRepository = dbRepositoryMock.Object;

            var mockExchangeRatesLoader = new Mock<ExchangeRatesLoader>(new Mock<ILogger<ExchangeRatesLoader>>().Object, new Mock<IOpenExchangeRatesClient>().Object);
            mockExchangeRatesLoader.Setup(x => x.GetCurrencyRates(_from, _to)).ReturnsAsync(apiRates);
            var _exchangeRatesLoader = mockExchangeRatesLoader.Object;

            ExchangeRatesLoaderWithCaching exchangeRatesLoader = new(_exchangeRatesLoader, _dbRepository);
            //assert

            var result = await exchangeRatesLoader.GetCurrencyRates(_from, _to);

            //act
            Assert.Equal(JsonConvert.SerializeObject(cachedRates), JsonConvert.SerializeObject(result));
        }
        [Fact()]
        public async void GetCurrencyRatesTest_AllRatesFromCacheAreMissing_ShouldReturnOk()
        {
            //arrange
            DateTime _from = new(2022, 11, 11);
            DateTime _to = new(2022, 11, 12);
            List<CurrencyRateModel> cachedRates = new();
            List<CurrencyRateModel> apiRates = new()
            {
                new CurrencyRateModel(){DateTime=new(2022, 11, 11), Rates=new(){ {"eur",(decimal)1.05 } } },
                new CurrencyRateModel(){DateTime=new(2022, 11, 12), Rates=new(){ {"eur",(decimal)0.9 } } },
            };


            var dbRepositoryMock = new Mock<IDbRepository>();
            dbRepositoryMock.Setup(x => x.GetRates(_from, _to)).ReturnsAsync(cachedRates);
            var _dbRepository = dbRepositoryMock.Object;

            var mockExchangeRatesLoader = new Mock<ExchangeRatesLoader>(new Mock<ILogger<ExchangeRatesLoader>>().Object, new Mock<IOpenExchangeRatesClient>().Object);
            mockExchangeRatesLoader.Setup(x => x.GetCurrencyRates(_from, _to)).ReturnsAsync(apiRates);
            var _exchangeRatesLoader = mockExchangeRatesLoader.Object;

            ExchangeRatesLoaderWithCaching exchangeRatesLoader = new(_exchangeRatesLoader, _dbRepository);
            //assert

            var result = await exchangeRatesLoader.GetCurrencyRates(_from, _to);

            //act
            Assert.Equal(JsonConvert.SerializeObject(apiRates), JsonConvert.SerializeObject(result));
        }
        [Fact()]
        public async void GetCurrencyRatesTest_SecondPartIsMissing_ShouldReturnOk()
        {
            //arrange
            DateTime _fromRepository = new(2022, 11, 11);
            DateTime _toRepository = new(2022, 11, 11);
            DateTime _fromApi = new(2022, 11, 12);
            DateTime _toApi = new(2022, 11, 12);
            List<CurrencyRateModel> cachedRates = new()
            {
                new CurrencyRateModel(){DateTime=new(2022, 11, 11), Rates=new(){ {"eur",(decimal)0.9 } } },
            };
            List<CurrencyRateModel> apiRates = new()
            {
                new CurrencyRateModel(){DateTime=new(2022, 11, 12), Rates=new(){ {"eur",(decimal)1.1} } },
            };


            var dbRepositoryMock = new Mock<IDbRepository>();
            dbRepositoryMock.Setup(x => x.GetRates(_fromRepository, _toApi)).ReturnsAsync(cachedRates);
            var _dbRepository = dbRepositoryMock.Object;

            var mockExchangeRatesLoader = new Mock<ExchangeRatesLoader>(new Mock<ILogger<ExchangeRatesLoader>>().Object, new Mock<IOpenExchangeRatesClient>().Object);
            mockExchangeRatesLoader.Setup(x => x.GetCurrencyRates(_fromApi, _toApi)).ReturnsAsync(apiRates);
            var _exchangeRatesLoader = mockExchangeRatesLoader.Object;

            ExchangeRatesLoaderWithCaching exchangeRatesLoader = new(_exchangeRatesLoader, _dbRepository);
            //assert

            var result = await exchangeRatesLoader.GetCurrencyRates(_fromRepository, _toApi);

            //act
            Assert.Equal(JsonConvert.SerializeObject(cachedRates.Concat(apiRates).ToList()), JsonConvert.SerializeObject(result));
        }
        [Fact()]
        public async void GetCurrencyRatesTest_FirstPartIsMissing_ShouldReturnOk()
        {
            //arrange
            DateTime _fromRepository = new(2022, 11, 12);
            DateTime _toRepository = new(2022, 11, 12);
            DateTime _fromApi = new(2022, 11, 11);
            DateTime _toApi = new(2022, 11, 11);
            List<CurrencyRateModel> cachedRates = new()
            {
                new CurrencyRateModel(){DateTime=new(2022, 11, 12), Rates=new(){ {"eur",(decimal)0.9 } } },
            };
            List<CurrencyRateModel> apiRates = new()
            {
                new CurrencyRateModel(){DateTime=new(2022, 11, 11), Rates=new(){ {"eur",(decimal)1.1} } },
            };


            var dbRepositoryMock = new Mock<IDbRepository>();
            dbRepositoryMock.Setup(x => x.GetRates(_fromApi, _toRepository)).ReturnsAsync(cachedRates);
            var _dbRepository = dbRepositoryMock.Object;

            var mockExchangeRatesLoader = new Mock<ExchangeRatesLoader>(new Mock<ILogger<ExchangeRatesLoader>>().Object, new Mock<IOpenExchangeRatesClient>().Object);
            mockExchangeRatesLoader.Setup(x => x.GetCurrencyRates(_fromApi, _toApi)).ReturnsAsync(apiRates);
            var _exchangeRatesLoader = mockExchangeRatesLoader.Object;

            ExchangeRatesLoaderWithCaching exchangeRatesLoader = new(_exchangeRatesLoader, _dbRepository);
            //assert

            var result = await exchangeRatesLoader.GetCurrencyRates(_fromApi, _toRepository);

            //act
            Assert.Equal(JsonConvert.SerializeObject(apiRates.Concat(cachedRates).ToList()), JsonConvert.SerializeObject(result));
        }
        [Fact()]
        public async void GetCurrencyRatesTest_FirstAndSecondPartsAreMissing_ShouldReturnOk()
        {

            //arrange
            DateTime _fromRepository = new(2022, 11, 12);
            DateTime _toRepository = new(2022, 11, 12);
            DateTime _fromApiFirst = new(2022, 11, 11);
            DateTime _toApiFirst = new(2022, 11, 11);
            DateTime _fromApiSecond = new(2022, 11, 13);
            DateTime _toApiSecond = new(2022, 11, 13);
            List<CurrencyRateModel> cachedRates = new()
            {
                new CurrencyRateModel(){DateTime=new(2022, 11, 12), Rates=new(){ {"eur",(decimal)0.9 } } },
            };
            List<CurrencyRateModel> apiRatesFirst = new()
            {
                new CurrencyRateModel(){DateTime=new(2022, 11, 11), Rates=new(){ {"eur",(decimal)1.1} } },
            };
            List<CurrencyRateModel> apiRatesSecond = new()
            {
                new CurrencyRateModel(){DateTime=new(2022, 11, 13), Rates=new(){ {"eur",(decimal)1.3} } },
            };

            var dbRepositoryMock = new Mock<IDbRepository>();
            dbRepositoryMock.Setup(x => x.GetRates(_fromApiFirst, _toApiSecond)).ReturnsAsync(cachedRates);
            var _dbRepository = dbRepositoryMock.Object;

            var mockExchangeRatesLoader = new Mock<ExchangeRatesLoader>(new Mock<ILogger<ExchangeRatesLoader>>().Object, new Mock<IOpenExchangeRatesClient>().Object);
            mockExchangeRatesLoader.Setup(x => x.GetCurrencyRates(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync((DateTime x, DateTime y) =>
            {
                if (x == _fromApiFirst && y == _toApiFirst)
                {
                    return apiRatesFirst;
                }
                else
                {
                    return apiRatesSecond;
                }
            });
            var _exchangeRatesLoader = mockExchangeRatesLoader.Object;

            ExchangeRatesLoaderWithCaching exchangeRatesLoader = new(_exchangeRatesLoader, _dbRepository);
            //assert

            var result = await exchangeRatesLoader.GetCurrencyRates(_fromApiFirst, _toApiSecond);

            //act
            Assert.Equal(JsonConvert.SerializeObject(apiRatesFirst.Concat(cachedRates ).Concat(apiRatesSecond).ToList()), JsonConvert.SerializeObject(result));

        }
    }
}