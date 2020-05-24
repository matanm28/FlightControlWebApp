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
    using System.Diagnostics;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlansController : ControllerBase {
        private readonly FlightControlContext _context;
        private const int ModNumber = 100000;

        public FlightPlansController(FlightControlContext context) {
            _context = context;
        }

        // GET: api/FlightPlans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightPlans() {
            return await _context.FlightPlans.ToListAsync();
        }

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(int id) {
            var flightPlan = await _context.FlightPlans
                                     .Include(flightPlan => flightPlan.Segments)
                                     .Include(flightPlan => flightPlan.InitialLocation)
                                     .FirstAsync(flightPlan => flightPlan.Id == id);
            if (flightPlan == null) {
                return NotFound();
            }

            return Ok(flightPlan);
        }

        // PUT: api/FlightPlans/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightPlan(int id, FlightPlan flightPlan) {
            if (id != flightPlan.Id) {
                return BadRequest();
            }

            _context.Entry(flightPlan).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!FlightPlanExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan(FlightPlan flightPlan) {
            Flight flight = new Flight()
                                {
                                    FlightId = GenerateFlightId(flightPlan),
                                    Longitude = flightPlan.InitialLocation.Longitude,
                                    Latitude = flightPlan.InitialLocation.Latitude,
                                    Passengers = flightPlan.Passengers,
                                    CompanyName = flightPlan.CompanyName,
                                    DateTime = flightPlan.InitialLocation.DateTime,
                                };
            var sendRequest = await this.postFlight(flight);
            if (!sendRequest.IsSuccessStatusCode) {
                Debug.WriteLine(sendRequest);
                return BadRequest(flight);
            }
            _context.FlightPlans.Add(flightPlan);
            await _context.SaveChangesAsync();

            var response = CreatedAtAction("GetFlightPlan", new { id = flightPlan.Id }, flightPlan);
            return response;
        }

        [HttpPost("List")]
        public async Task<ActionResult<IEnumerable<FlightPlan>>> PostFlightPlansList(IEnumerable<FlightPlan> flightPlans) {
            foreach (FlightPlan flightPlan in flightPlans) {
                var actionResult = await PostFlightPlan(flightPlan);
                if (!(actionResult.Result is CreatedAtActionResult)) {
                    return BadRequest(flightPlan);
                }
            }

            return Created(nameof(GetFlightPlans), flightPlans);
        }

        // DELETE: api/FlightPlans/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightPlan>> DeleteFlightPlan(int id) {
            var flightPlan = await _context.FlightPlans.FindAsync(id);
            if (flightPlan == null) {
                return NotFound();
            }

            _context.FlightPlans.Remove(flightPlan);
            await _context.SaveChangesAsync();

            return Ok(flightPlan);
        }

        private bool FlightPlanExists(int id) {
            return _context.FlightPlans.Any(e => e.Id == id);
        }

        private string calcFlightId(FlightPlan flightPlan) {
            StringBuilder sb = new StringBuilder();
            using (SHA256 hashSha256 = SHA256.Create()) {
                var hashedValue = hashSha256.ComputeHash(Encoding.UTF8.GetBytes(flightPlan.CompanyName + flightPlan.Passengers
                                                                                                       + flightPlan.InitialLocation.DateTime
                                                                                                       + RandomNumberGenerator.GetInt32(int.MaxValue)));
                int mod = ModNumber;
                int length = flightPlan.CompanyName.Length;
                if (length < 5) {
                    for (int i = 0; i < 5 - length; i++) {
                        mod *= 10;
                    }

                    sb.Append(flightPlan.CompanyName.Substring(0, length));
                } else {
                    sb.Append(flightPlan.CompanyName.Substring(0, 5));
                }

                // var key = BitConverter.ToUInt64(hashedValue) % mod;
                var key = Math.Abs(flightPlan.GetHashCode()) % mod;
                sb.Append($"-{key}");
            }

            return sb.ToString().ToUpper();
        }

        private async Task<HttpResponseMessage> postFlight(Flight flight) {
            using (HttpClient client = new HttpClient()) {
                var check = this.Request.Host.Value;
                var uri = new Uri("https://" + this.Request.Host.Value + "/api/Flights");
                string contents = JsonSerializer.Serialize(flight);
                var response = client.PostAsync(uri, new StringContent(contents, Encoding.UTF8, "application/json"));
                var result = await response.ConfigureAwait(false);
                if (!result.IsSuccessStatusCode) {
                    return result;
                }

                return result;
            }
        }

        internal static string GenerateFlightId(FlightPlan flightPlan) {
            StringBuilder sb = new StringBuilder();
            int mod = ModNumber;
            int length = flightPlan.CompanyName.Length;
            if (length < 5) {
                for (int i = 0; i < 5 - length; i++) {
                    mod *= 10;
                }
                sb.Append(flightPlan.CompanyName.Substring(0, length));
            } else {
                sb.Append(flightPlan.CompanyName.Substring(0, 5));
            }

            // var key = BitConverter.ToUInt64(hashedValue) % mod;
            var key = Math.Abs(flightPlan.GetHashCode()) % mod;
            return sb.Append($"-{key}").ToString().ToUpper();
        }
    }
}
