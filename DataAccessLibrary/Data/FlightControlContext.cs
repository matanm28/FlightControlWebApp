using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Data {
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;

    public class FlightControlContext : DbContext {

        public DbSet<FlightPlan> FlightPlans { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Segments> Segmentses { get; set; }
        public DbSet<ApiServer> ApiServer { get; set; }

        /// <inheritdoc />
        public FlightControlContext(DbContextOptions options)
            : base(options) {
        }
    }
}