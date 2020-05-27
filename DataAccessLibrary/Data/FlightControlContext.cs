using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Data {
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;

    public class FlightControlContext : DbContext {
        public DbSet<FlightPlan> FlightPlans { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Segment> Segmentses { get; set; }
        public DbSet<Server> Servers { get; set; }


        /// <inheritdoc />
        public FlightControlContext(DbContextOptions options)
                : base(options) {
            
        }

    }
}
