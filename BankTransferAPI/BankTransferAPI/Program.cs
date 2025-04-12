using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using BankingApi.Services;
using BankTransferAPI.Interfaces;
using BankTransferAPI.Repositories;
using BankingApi.Repositories;
using Microsoft.AspNetCore.Hosting;

namespace BankingApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddControllers();
                    services.AddTransient<ITransferService, TransferenciaBancariaService>();
                    services.AddTransient<ITransferRepository, TransferRepository>();
                    services.AddTransient<IClienteRepository, ClienteRepository>();
                    services.AddControllers().AddApplicationPart(typeof(ClienteController).Assembly);

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
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure(app =>
                    {
                        // Swagger first
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
                    });
                });
    }
}
