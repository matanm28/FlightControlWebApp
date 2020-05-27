namespace DataAccessLibrary.DataAccess.Interfaces {
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;

    public interface IServerService : IDataAccess<Server,int> {
        public DbSet<Server> ServerDbSet { get; }

    }
}
