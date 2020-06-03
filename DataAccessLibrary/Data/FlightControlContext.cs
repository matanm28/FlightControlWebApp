namespace DataAccessLibrary.Data {
    using System.Diagnostics.CodeAnalysis;
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;

    public class FlightControlContext : DbContext {

        public DbSet<FlightPlan> FlightPlans { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Segment> Segments { get; set; }
        public DbSet<Server> Servers { get; set; }


        [SuppressMessage("Compiler", "CS8618")]
        /// <inheritdoc />
        public FlightControlContext(DbContextOptions options)
                : base(options) {
            
        }

    }
}
