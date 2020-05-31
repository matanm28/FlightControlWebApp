namespace FlightControlWeb.Controllers.Interfaces {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccessLibrary.Models;
    using Microsoft.AspNetCore.Mvc;

    public interface IFlightPlanController {
        Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightPlans();

        Task<ActionResult<FlightPlan>> GetFlightPlan(string id);

        Task<ActionResult<FlightPlan>> PostFlightPlan(FlightPlan flightPlan);

        Task<ActionResult<IEnumerable<FlightPlan>>> PostFlightPlansList(IEnumerable<FlightPlan> flightPlans);
    }
}
