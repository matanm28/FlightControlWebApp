namespace DataAccessLibrary.DataAccess.Interfaces {
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;

    public interface IServerService : IDataAccess<Server,string> {
        public DbSet<Server> ServerDbSet { get; }

        Task<Server> FindAsync(Server server);

    }
}
