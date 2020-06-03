namespace DataAccessLibrary.Data {
    using System.Configuration;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    public class FlightControlContextFactory : IDesignTimeDbContextFactory<FlightControlContext> {
        /// <inheritdoc />
        public FlightControlContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<FlightControlContext>();
            optionsBuilder.UseSqlite("data source=FlightControlDB.sqlite;");

            return new FlightControlContext(optionsBuilder.Options);
        }
    }
}
