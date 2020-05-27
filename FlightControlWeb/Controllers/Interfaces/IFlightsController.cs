namespace FlightControlWeb.Controllers {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccessLibrary.Models;
    using Microsoft.AspNetCore.Mvc;

    public interface IFlightsController {
        Task<ActionResult<IEnumerable<Flight>>> GetFlights();

        Task<ActionResult<Flight>> GetFlight(string id);

        Task<ActionResult<Flight>> DeleteFlight(string id);

        Task<ActionResult<IEnumerable<Flight>>> GetFlightsRelativeTo([FromQuery] DateTime relative_to);
    }
}
