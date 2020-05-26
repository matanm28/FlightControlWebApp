using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Data {
    using System.Data.Entity.Infrastructure;
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;

    public interface IFlightControlContext: IDisposable, IObjectContextAdapter {
        public DbSet<FlightPlan> FlightPlans { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Segment> Segmentses { get; set; }
        public DbSet<Server> ApiServer { get; set; }
    }
}
