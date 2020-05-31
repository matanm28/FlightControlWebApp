using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlightControlWeb {
    using System;
    using System.Reflection;
    using Autofac;
    using Autofac.Integration.WebApi;
    using DataAccessLibrary.Data;
    using DataAccessLibrary.DataAccess.Implementations;
    using DataAccessLibrary.DataAccess.Interfaces;
    
    using FlightControlWeb.Controllers.Implementations;
    using FlightControlWeb.Controllers.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Startup {
        private const int SecondsToTimeOut = 5;
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
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
        }

        public void ConfigureContainer(ContainerBuilder builder) {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<FlightControlContext>().As<DbContext>();
            builder.RegisterType<FlightPlansService>().As<IFlightPlansService>();
            builder.RegisterType<ServerService>().As<IServerService>();
            builder.RegisterType<FlightPlanController>().As<IFlightPlanController>();
            builder.RegisterType<ServersController>().As<IServersController>();
            builder.RegisterType<FlightsController>().As<IFlightsController>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
