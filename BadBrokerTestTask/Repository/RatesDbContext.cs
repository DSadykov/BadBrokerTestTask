using System.Collections.Generic;
using System.Threading.Tasks;

using BadBrokerTestTask.Models;

using Microsoft.EntityFrameworkCore;

namespace BadBrokerTestTask.Repository
{
    public class RatesDbContext: DbContext
    {
        public RatesDbContext(DbContextOptions<RatesDbContext> options)
            : base(options)
        {
        }
        public DbSet<CurrencyRateDbModel> CurrencyRateModels { get; set; }     
        public async Task AddRateModel(CurrencyRateDbModel currencyRateDbModel)
        {
            await CurrencyRateModels.AddAsync(currencyRateDbModel);
            await SaveChangesAsync();
        }
        public async Task AddRateModels(IEnumerable<CurrencyRateDbModel> currencyRateDbModels)
        {
            await CurrencyRateModels.AddRangeAsync(currencyRateDbModels);
            await SaveChangesAsync();
        }
    }
}
