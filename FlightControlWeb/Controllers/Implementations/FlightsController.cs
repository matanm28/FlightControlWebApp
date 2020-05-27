using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.Controllers {
    using System.Collections;
    using System.ComponentModel;
    using System.IO;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Security.Policy;
    using System.Text;
    using System.Threading;
    using DataAccessLibrary.Converters;
    using DataAccessLibrary.DataAccess.Enums;
    using DataAccessLibrary.DataAccess.Interfaces;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase, IFlightsController {
        private readonly IFlightPlansService flightPlansService;
        private readonly IServerService serverService;

        /// <inheritdoc />
        public FlightsController(IFlightPlansService flightPlansService, IServerService serverService) {
            this.flightPlansService = flightPlansService;
            this.serverService = serverService;
        }

        // GET: api/Flights
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights() {
            IList<Flight> flights = new List<Flight>();
            var flightPlans = await this.flightPlansService.GetAllAsync(FlightPlansProperty.IntialLocation);
            foreach (FlightPlan flightPlan in flightPlans) {
                if (flightPlan != null) {
                    flights.Add(new Flight(flightPlan));
                }
            }

            return Ok(flights);
        }

        // GET: api/Flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(string id) {
            var flightPlan = await this.flightPlansService.FindAsync(id);
            if (flightPlan == null) {
                return NotFound();
            }

            return Ok(new Flight(flightPlan));
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(string id) {
            if (!await this.flightPlansService.ExistsAsync(id)) {
                return NotFound();
            }

            var flightPlan = await this.flightPlansService.RemoveAsync(id);
            await this.flightPlansService.SaveChangesAsync();

            return Ok(new Flight(flightPlan));
        }

        // GET: api/Flights?relative_to=<DateTime>&sync_all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlightsRelativeTo([FromQuery] DateTime relative_to) {
            if (relative_to.Kind != DateTimeKind.Utc) {
                return BadRequest($"requset:\"{this.Request}\"\nstatus:\"failed\"\nreason:\"relative_to format must be yyyy-MM-ddTHH:mm:ssZ (UTC time)\"");
            }

            var flightPlansList = await this.flightPlansService.GetAllAsync(flight => relative_to >= flight.InitialLocation.DateTime);
            IList<Task<Flight?>> flightTasks = new List<Task<Flight?>>();
            foreach (FlightPlan flightPlan in flightPlansList) {
                flightTasks.Add(Flight.GetFlightRelativeToTimeAsync(flightPlan, relative_to));
            }

            List<Flight?> flightList = new List<Flight?>(await Task.WhenAll(flightTasks));

            if (this.Request.Query.Keys.Contains("sync_all")) {
                IList<Flight> externalFlights = await this.getFlightsFromExternalServersAsync(relative_to);
                flightList.AddRange(externalFlights);
            }

            flightList.RemoveAll(plan => plan == null);
            return Ok(flightList);
        }

        private async Task<IList<Flight>> getFlightsFromExternalServersAsync(DateTime relative_to) {
            var servers = await this.serverService.GetAllAsync();
            IList<Task<HttpResponseMessage>> tasksList = new List<Task<HttpResponseMessage>>();
            HttpClient client = new HttpClient();
            foreach (Server server in servers) {
                var uri = new Uri($"{server.URL}/api/Flights?relative_to={relative_to.ToString("yyyy-MM-ddTHH:mm:ssZ")}");
                tasksList.Add(client.GetAsync(uri));
            }

            List<Flight> flightsList = new List<Flight>();
            IList<HttpResponseMessage> responseList = await Task.WhenAll(tasksList);
            client.Dispose();
            foreach (HttpResponseMessage response in responseList) {
                try {
                    IEnumerable<Flight> tempFlights = JsonConvert.DeserializeObject<IEnumerable<Flight>>(await response.Content.ReadAsStringAsync());
                    if (tempFlights != null) {
                        flightsList.AddRange(tempFlights);
                    }
                }
                catch (Exception e) {
                    //todo add logger
                    continue;
                }
            }

            flightsList.ForEach(flight => flight.IsExternal = true);

            return flightsList;
        }

    }
}
