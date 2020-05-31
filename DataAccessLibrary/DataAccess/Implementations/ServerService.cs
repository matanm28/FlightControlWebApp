namespace DataAccessLibrary.DataAccess.Implementations {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using DataAccessLibrary.Data;
    using DataAccessLibrary.DataAccess.Interfaces;
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public class ServerService : IServerService {
        private readonly FlightControlContext dbContext;

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
        public async Task<Server> FindAsync(int id) {
            return await this.dbContext.Servers.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int id) {
            return (await this.FindAsync(id) != null);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(Server element) {
            if (element == null || element.URL == null) {
                return false;
            }

            return (await this.dbContext.Servers.AnyAsync(server => server.URL == element.URL)) != null;
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync() {
            return this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Server> RemoveAsync(Server element) {
            if (element != null) {
                Server server = this.dbContext.Servers.Remove(element).Entity;
                if (server.Equals(element)) {
                    await this.SaveChangesAsync();
                    return server;
                }
            }
            return null;
        }

        /// <inheritdoc />
        public async Task<Server> RemoveAsync(int id) {
            Server server = await this.dbContext.Servers.FindAsync(id);
            if (server == null) {
                return null;
            }

            this.dbContext.Remove(server);
            return server;
        }

        /// <inheritdoc />
        public DbSet<Server> ServerDbSet => this.dbContext.Servers;

        private bool validateServer(Server server) {
            foreach (PropertyInfo propertyInfo in server.GetType().GetProperties()) {
                if (propertyInfo.Name == nameof(Server.Id)) {
                    continue;
                }
                object value = propertyInfo.GetValue(server);
                if (value == null) {
                    return false;
                }
            }
            return true;
        }
    }
}
