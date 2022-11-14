using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Models;

using Dapper;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BadBrokerTestTask.Repository
{
    public class DbRepository : IDbRepository
    {
        private readonly SqlConnectionStringBuilder _connectionBuilder;
        private readonly ILogger<DbRepository> _logger;

        public DbRepository(IConfiguration configuration, ILogger<DbRepository> logger)
        {
            _connectionBuilder = new SqlConnectionStringBuilder(configuration.GetConnectionString("Default"));
            _logger = logger;
            EnsureCreatedDb();
            EnsureCreatedTable();

        }

        private void EnsureCreatedTable()
        {
            using IDbConnection db = new SqlConnection(_connectionBuilder.ConnectionString);
            try
            {
                var sqlQuery = "SELECT TOP(1) * FROM Rates";
                db.Execute(sqlQuery);
            }
            catch (SqlException)
            {
                var sqlQuery = @"CREATE TABLE Rates( 
                    Id int identity(1,1) PRIMARY KEY, 
                    Currency nvarchar(20) , 
                    Date datetime not null , 
                    Rate decimal(10,6) NOT NULL 
                    )";
                db.Execute(sqlQuery);
            }
        }

        private void EnsureCreatedDb()
        {
            var dbName = _connectionBuilder.InitialCatalog;
            _connectionBuilder.InitialCatalog = "";
            using IDbConnection db = new SqlConnection(_connectionBuilder.ConnectionString);
            try
            {
                db.Open();
                db.ChangeDatabase(dbName);
            }
            catch (SqlException x)
            {
                if (x.Message.StartsWith($"Database '{dbName}' does not exist"))
                {
                    var sqlQuery = $"CREATE DATABASE {dbName}";
                    db.Execute(sqlQuery);
                }
                else
                {
                    _logger.LogError("Error while connecting to DB");
                    throw;
                }
            }
            _connectionBuilder.InitialCatalog = dbName;
        }

        public async Task<List<CurrencyRateModel>> GetRates(DateTime from, DateTime to)
        {
            using IDbConnection db = new SqlConnection(_connectionBuilder.ConnectionString);
            var sqlQuery = @"select [Currency]
      ,[Date]
      ,[Rate]
	  from [Rates]
	  where date between @from and @to
      order by date";

            var queryResult = await db.QueryAsync<CurrencyRateDbModel>(sqlQuery, new { from, to });
            var result = new List<CurrencyRateModel>();
            foreach (var groupedCurrencyRateDbModel in queryResult.GroupBy(x => x.Date))
            {
                var currencyRateModel = new CurrencyRateModel()
                {
                    DateTime = groupedCurrencyRateDbModel.Key,
                    Rates = new()
                };
                foreach (var currencyRateDbModel in groupedCurrencyRateDbModel)
                {
                    currencyRateModel.Rates[currencyRateDbModel.Currency] = currencyRateDbModel.Rate;
                }
                result.Add(currencyRateModel);
            }

            return result;
        }

        public async Task AddRates(List<CurrencyRateModel> currencyRateModels)
        {
            using IDbConnection db = new SqlConnection(_connectionBuilder.ConnectionString);
            var sqlQuery = @"if((select count(*) from Rates where Date=@date and @currency=Currency)=1)
	return
else
	INSERT INTO [dbo].[Rates]
	           ([Currency]
	           ,[Date]
	           ,[rate])
	     VALUES
	           (@currency
	           ,@date
	           ,@rate)";
            foreach (var currencyRateModel in currencyRateModels) 
            { 
                foreach(var rate in currencyRateModel.Rates)
                {
                    await db.ExecuteAsync(sqlQuery, new CurrencyRateDbModel() { Currency=rate.Key, Date=currencyRateModel.DateTime, Rate=rate.Value});
                }
            }
        }

    }
}