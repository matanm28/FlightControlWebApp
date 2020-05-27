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
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase {
        public enum Sync { sync_all, no_sync };

        private readonly FlightControlContext _context;

        public FlightsController(FlightControlContext context) {
            _context = context;
        }

        // GET: api/Flights
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights() {
            IList<Flight> flights = new List<Flight>();
            var flightPlans = await _context.FlightPlans.Include(nameof(FlightPlan.InitialLocation)).ToListAsync();
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
            var flightPlan = await _context.FlightPlans.Include(nameof(FlightPlan.InitialLocation)).Include(nameof(FlightPlan.Segments))
                                       .FirstOrDefaultAsync(x => x.Id == id);

            if (flightPlan == null) {
                return NotFound();
            }

            return Ok(new Flight(flightPlan));
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(string id) {
            var flightPlan = await _context.FlightPlans.Include(nameof(FlightPlan.InitialLocation)).Include(nameof(FlightPlan.Segments))
                                           .FirstOrDefaultAsync(x => x.Id == id);
            if (flightPlan == null) {
                return NotFound();
            }

            _context.FlightPlans.Remove(flightPlan);
            await _context.SaveChangesAsync();

            return Ok(new Flight(flightPlan));
        }

        // GET: api/Flights?relative_to=<DateTime>&sync_all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlightsRelativeTo([FromQuery] DateTime relative_to) {
            if (relative_to.Kind != DateTimeKind.Utc) {
                return BadRequest($"requset:\"{this.Request}\"\nstatus:\"failed\"\nreason:\"relative_to format must be yyyy-MM-ddTHH:mm:ssZ (UTC time)\"");
            }

            var flightPlansList = await this._context.FlightPlans.Include(x => x.InitialLocation).Include(x => x.Segments)
                                            .Where(flight => relative_to >= flight.InitialLocation.DateTime).ToListAsync();
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
            var servers = await this._context.ApiServer.ToListAsync();
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
                    continue;
                }
            }

            flightsList.ForEach(flight => flight.IsExternal = true);

            return flightsList;
        }

        private bool FlightExists(string id) {
            return _context.FlightPlans.Any(e => e.Id == id);
        }
    }
}
