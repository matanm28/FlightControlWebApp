namespace DataAccessLibrary.DataAccess.Implementations {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using DataAccessLibrary.Data;
    using DataAccessLibrary.DataAccess.Enums;
    using DataAccessLibrary.DataAccess.Interfaces;
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public class FlightPlansService : IFlightPlansService {
        private readonly FlightControlContext dbContext;

        public FlightPlansService(FlightControlContext dbContext) {
            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<EntityEntry<FlightPlan>> AddAsync(FlightPlan element) {
            return await this.dbContext.FlightPlans.AddAsync(element);
        }

        /// <inheritdoc />
        public Task<List<FlightPlan>> GetAllAsync() {
            return this.dbContext.FlightPlans.Include(nameof(FlightPlan.InitialLocation)).Include(nameof(FlightPlan.Segments)).ToListAsync();
        }

        /// <inheritdoc />
        public Task<List<FlightPlan>> GetAllAsync([NotNull] Expression<Func<FlightPlan, bool>> predicate) {
            return this.dbContext.FlightPlans.Include(nameof(FlightPlan.InitialLocation)).Include(nameof(FlightPlan.Segments)).Where(predicate).ToListAsync();
        }

        /// <inheritdoc />
        public Task<List<FlightPlan>> GetAllAsync(FlightPlansProperty property) {
            switch (property) {
                default:
                case FlightPlansProperty.All:
                    return this.GetAllAsync();
                case FlightPlansProperty.IntialLocation:
                    return this.dbContext.FlightPlans.Include(nameof(FlightPlan.InitialLocation)).ToListAsync();
                case FlightPlansProperty.Segments:
                    return this.dbContext.FlightPlans.Include(nameof(FlightPlan.Segments)).ToListAsync();
            }
        }

        /// <inheritdoc />
        public Task<FlightPlan> FindAsync(string id) {
            if (id == null) {
                return Task.FromResult<FlightPlan>(result: null);
            }

            return this.dbContext.FlightPlans.Include(nameof(FlightPlan.InitialLocation)).Include(nameof(FlightPlan.Segments))
                       .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string id) {
            return await this.FindAsync(id) != null;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(FlightPlan element) {
            if (element == null || !validateFlightPlan(element)) {
                return false;
            }

            return await this.dbContext.FlightPlans.AnyAsync(flightPlan => element.Equals(flightPlan));
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync() {
            return this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<FlightPlan> RemoveAsync(string id) {
            FlightPlan flightPlan = await this.FindAsync(id);
            if (flightPlan == null) {
                return null;
            }
            this.dbContext.FlightPlans.Remove(flightPlan);
            return flightPlan;
        }

        /// <inheritdoc />
        public async Task<FlightPlan> RemoveAsync(FlightPlan element) {
            if (element.Id != null) {
                return await this.RemoveAsync(element.Id);
            }
            return null;
        }

        /// <inheritdoc />
        public DbSet<FlightPlan> FlightPlansDbSet => this.dbContext.FlightPlans;

        private bool validateFlightPlan(FlightPlan flightPlan) {
            foreach (PropertyInfo propertyInfo in flightPlan.GetType().GetProperties()) {
                if (propertyInfo.Name == nameof(FlightPlan.Id)) {
                    continue;
                }
                object? value = propertyInfo.GetValue(flightPlan);
                if (value == null) {
                    return false;
                }
            }

            return true;
        }

    }
}
