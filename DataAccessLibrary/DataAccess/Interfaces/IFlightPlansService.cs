namespace DataAccessLibrary.DataAccess.Interfaces {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccessLibrary.DataAccess.Enums;
    using DataAccessLibrary.Models;
    using Microsoft.EntityFrameworkCore;

    public interface IFlightPlansService: IDataAccess<FlightPlan,string> {
        
        DbSet<FlightPlan> FlightPlansDbSet { get; }

        Task<List<FlightPlan>> GetAllAsync(FlightPlansProperty property);

    }
}