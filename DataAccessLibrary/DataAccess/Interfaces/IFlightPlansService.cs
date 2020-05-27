namespace DataAccessLibrary.DataAccess.Interfaces {
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;

    public interface IFlightPlansService: IDataAccess<FlightPlan,string> {

        public DbSet<FlightPlan> FlightPlansDbSet { get; }


    }
}