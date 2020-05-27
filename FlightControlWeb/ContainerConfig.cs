using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Integration.WebApi;
using System.Reflection;

namespace FlightControlWeb {
    using Autofac;
    using DataAccessLibrary.Data;

    public static class ContainerConfig {

        public static IContainer Configure() {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<FlightControlContext>().AsSelf();
            return builder.Build();
        }
    }
}
