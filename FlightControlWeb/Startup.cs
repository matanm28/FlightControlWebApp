using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlightControlWeb {
    using System;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using DataAccessLibrary.Data;
    using DataAccessLibrary.DataAccess.Implementations;
    using DataAccessLibrary.DataAccess.Interfaces;
    using FlightControlWeb.Controllers.Implementations;
    using FlightControlWeb.Controllers.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Startup {
        private const int SecondsToTimeOut = 10;
        public Startup(IWebHostEnvironment env) {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                                                    .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; private set; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<FlightControlContext>(
                    options => {
                        options.UseSqlite(Configuration.GetConnectionString("FlightControlDB"));
                    });

            services.AddControllers().AddNewtonsoftJson(
                    options => {
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    });
            services.AddHttpClient(nameof(IServerService),
                    client => {
                        client.Timeout = TimeSpan.FromSeconds(SecondsToTimeOut);
                    });
            services.AddMvc().AddControllersAsServices();
            services.AddOptions();
        }

        public void ConfigureContainer(ContainerBuilder builder) {
            builder.RegisterType<FlightControlContext>().As<DbContext>();
            builder.RegisterType<FlightPlansService>().As<IFlightPlansService>();
            builder.RegisterType<ServerService>().As<IServerService>();
            builder.RegisterType<FlightPlanController>().As<IFlightPlanController>().InstancePerRequest();
            builder.RegisterType<ServersController>().As<IServersController>().InstancePerRequest();
            builder.RegisterType<FlightsController>().As<IFlightsController>().InstancePerRequest();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
            
        }
    }
}
