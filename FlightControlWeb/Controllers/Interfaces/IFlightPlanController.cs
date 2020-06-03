namespace FlightControlWeb.Controllers.Interfaces {

    using DataAccessLibrary.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFlightPlanController {

        Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightPlans();

        Task<ActionResult<FlightPlan>> GetFlightPlan(string id);

        Task<ActionResult<FlightPlan>> PostFlightPlan(FlightPlan flightPlan);

        Task<ActionResult<IEnumerable<FlightPlan>>> PostFlightPlansList(IEnumerable<FlightPlan> flightPlans);
    }
}