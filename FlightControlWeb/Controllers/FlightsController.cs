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
            return await _context.Flights.ToListAsync();
        }

        // GET: api/Flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(string id) {
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null) {
                return NotFound();
            }

            return flight;
        }

        // PUT: api/Flights/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlight(string id, Flight flight) {
            if (id != flight.FlightId) {
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!FlightExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Flights
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight) {
            flight.IsExternal = false;
            _context.Flights.Add(flight);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (FlightExists(flight.FlightId)) {
                    return Conflict();
                } else {
                    throw;
                }
            }

            return CreatedAtAction("GetFlight",
                    new
                        {
                            id = flight.FlightId
                        },
                    flight);
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(string id) {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) {
                return NotFound();
            }

            var flightPlan = await this._context.FlightPlans.Include(x => x.InitialLocation).Include(x => x.Segments).FirstOrDefaultAsync();
            if (flightPlan == null) {
                return NotFound();
            }

            _context.Flights.Remove(flight);
            _context.FlightPlans.Remove(flightPlan);
            await _context.SaveChangesAsync();

            return flight;
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
                flightTasks.Add(flightPlan.GetFlightRelativeToTimeAsync(relative_to));
            }

            List<Flight?> flightList = new List<Flight?>(await Task.WhenAll(flightTasks));

            if (this.Request.Query.Keys.Contains("sync_all")) {
                IList<Flight> externalFlights = await this.getFlightsFromExternalServersAsync(relative_to);
                flightList.AddRange(externalFlights);
            }

            flightList.RemoveAll(plan => plan == null);
            return flightList;
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
                } catch (Exception e) {
                    continue;
                }

            }

            flightsList.ForEach(flight => flight.IsExternal = true);

            return flightsList;
        }

        private bool FlightExists(string id) {
            return _context.Flights.Any(e => e.FlightId == id);
        }
    }
}
