using System;

using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Middleware;
using BadBrokerTestTask.Repository;
using BadBrokerTestTask.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BadBrokerTestTask
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BadBrokerTestTask", Version = "v1" });
            });
            services.AddDbContext<RatesDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddTransient<ExchangeRatesLoader>();
            services.AddTransient<IExchangeRatesLoader, ExchangeRatesLoaderWithCaching>();
            services.AddTransient<IOpenExchangeRatesClient, OpenExchangeRatesClient>();
            services.AddTransient<IRevenueCalculator, RevenueCalculator>();
            services.AddTransient<ExceptionHandlingMiddleware>();
            services.AddSingleton<IDbRepository, DbRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BadBrokerTestTask v1"));
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
