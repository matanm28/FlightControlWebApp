using Autofac.Extensions.DependencyInjection;
using DataAccessLibrary.Converters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.IO;

namespace FlightControlWeb {

    public class Program {

        public static void Main(string[] args) {
            var host = Host.CreateDefaultBuilder(args).UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureWebHostDefaults(
                    webHostBuilder => {
                        webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory()).UseIISIntegration().UseStartup<Startup>();
                    }).Build();
            TypeDescriptor.AddAttributes(typeof(DateTime), new TypeConverterAttribute(typeof(UtcDateTimeConverter)));
            createDbIfNotExistsAsync(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(
                        webBuilder => {
                            webBuilder.UseStartup<Startup>();
                        });

        private static void createDbIfNotExistsAsync(IHost host) {
            using (var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;

                try {
                    var context = services.GetRequiredService<DbContext>();
                    context.Database.Migrate();
                    //context.Database.EnsureCreated();

                } catch (Exception ex) {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}