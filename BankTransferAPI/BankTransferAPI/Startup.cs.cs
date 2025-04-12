using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using BankingApi.Repositories;
using BankingApi.Services;
using BankTransferAPI.Interfaces;
using BankTransferAPI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BankingApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDefaultAWSOptions(_configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>();

            services.AddControllers();
            services.AddTransient<ITransferService, TransferenciaBancariaService>();
            services.AddTransient<ITransferRepository, TransferRepository>();
            services.AddTransient<IClienteRepository, ClienteRepository>();
            services.AddControllers().AddApplicationPart(typeof(TransferenciaController).Assembly);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Banking API",
                    Version = "v1",
                    Description = "Predo Caixa Pequena",
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
