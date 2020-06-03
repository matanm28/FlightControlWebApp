namespace DataAccessLibrary.DataAccess.Implementations {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using DataAccessLibrary.Data;
    using DataAccessLibrary.DataAccess.Interfaces;
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;


    /// <summary>
    /// Data base access services for Servers
    /// </summary>
    /// <seealso cref="DataAccessLibrary.DataAccess.Interfaces.IServerService" />
    public class ServerService : IServerService {
        private readonly FlightControlContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public ServerService(FlightControlContext dbContext) {
            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<EntityEntry<Server>> AddAsync(Server element) {
            return await this.dbContext.Servers.AddAsync(element);
        }

        /// <inheritdoc />
        public Task<List<Server>> GetAllAsync() {
            return this.dbContext.Servers.ToListAsync();
        }

        /// <inheritdoc />
        public Task<List<Server>> GetAllAsync(Expression<Func<Server, bool>> predicate) {
            return this.dbContext.Servers.Where(predicate).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Server> FindAsync(string id) {
            return await this.dbContext.Servers.FindAsync(id);
        }

        /// <inheritdoc />
        public Task<Server> FindAsync(Server server) {
            return this.dbContext.Servers.Where(x => x.URL == server.URL).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string id) {
            return await this.FindAsync(id) != null;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(Server element) {
            if (element?.URL == null) {
                return false;
            }
            return await this.dbContext.Servers.AnyAsync(server => server.URL == element.URL);
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync() {
            return this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Server> RemoveAsync(Server element) {
            if (element != null) {
                Server server = await this.dbContext.Servers.Where(x=>x.URL==element.URL).FirstOrDefaultAsync();
                if (server.Equals(element)) {
                    this.dbContext.Servers.Remove(server);
                    return server;
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <inheritdoc />
        [SuppressMessage("Compiler", "CS8603")]
        public async Task<Server> RemoveAsync(string id) {
            Server server = await this.dbContext.Servers.FindAsync(id);
            if (server == null) {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            this.dbContext.Remove(server);
            return server;
        }

        /// <inheritdoc />
        public DbSet<Server> ServerDbSet => this.dbContext.Servers;

        private bool validateServer(Server server) {
            foreach (PropertyInfo propertyInfo in server.GetType().GetProperties()) {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                object value = propertyInfo.GetValue(server);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (value == null) {
                    return false;
                }
            }
            return true;
        }
    }
}
