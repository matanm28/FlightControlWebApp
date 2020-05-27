namespace DataAccessLibrary.DataAccess.Interfaces {
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public interface IDataAccess<T, S> where T : class, new() where S : IComparable<S> {
        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAllAsync([NotNull] Expression<Func<T, bool>> predicate);

        Task<T?> FindAsync(S id);

        Task<bool> ExistsAsync(S id);

        Task<bool> ExistsAsync(T element);

        Task<int> SaveChangesAsync();

        Task<T?> RemoveAsync(T element);

        Task<T?> RemoveAsync(S id);

        Task<EntityEntry<T>> AddAsync(T element);


    }
}
