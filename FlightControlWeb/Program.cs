using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FlightControlWeb
{
    using System.ComponentModel;
    using System.IO;
    using DataAccessLibrary.Converters;
    using DataAccessLibrary.Data;
        using DataAccessLibrary.Models;
        using Microsoft.CodeAnalysis.CSharp.Syntax;
        using Microsoft.EntityFrameworkCore;
        using Microsoft.Extensions.DependencyInjection;

        public class Program
        {
                public static void Main(string[] args)
                {

                        var host = CreateHostBuilder(args).Build();
                        TypeDescriptor.AddAttributes(typeof(DateTime), new TypeConverterAttribute(typeof(UtcDateTimeConverter)));
                        CreateDbIfNotExists(host);
                        host.Run();
                }

                public static IHostBuilder CreateHostBuilder(string[] args) =>
                        Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
                        {
                                webBuilder.UseStartup<Startup>();

                        });

                private static void CreateDbIfNotExists(IHost host)
                {
                        using (var scope = host.Services.CreateScope()) {
                                var services = scope.ServiceProvider;

                                try {
                                        var context = services.GetRequiredService<FlightControlContext>();
                                        context.Database.EnsureCreated();
                                        context.Database.Migrate();
                                }
                                catch (Exception ex) {
                                        var logger = services.GetRequiredService<ILogger<Program>>();
                                        logger.LogError(ex, "An error occurred creating the DB.");
                                }
                        }
                }
        }
}