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
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using Castle.Core.Internal;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase {
        private static ISet<string> flightPlansId = new HashSet<string>();
        private const int ModNumber = 100000;
        private readonly FlightControlContext context;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ServersController serversController;

        public FlightPlanController(FlightControlContext context, ServersController serversController, IHttpClientFactory httpClientFactory) {
            this.context = context;
            this.httpClientFactory = httpClientFactory;
            this.serversController = serversController;
        }

        // GET: api/FlightPlans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightPlans() {
            return await this.context.FlightPlans.Include(x => x.Segments).Include(x => x.InitialLocation).ToListAsync();
        }

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id) {
            if (flightPlansId.Contains(id)) {
                return NotFound(id);
            }

            var flightPlan = (await GetFlightPlan(id)).Value;
            if (flightPlan != null) {
                return Ok(flightPlan);
            }

            var serversList = (await this.serversController.GetApiServer()).Value;
            if (serversList.IsNullOrEmpty()) {
                return NotFound(id);
            }

            flightPlansId.Add(id);
            using (HttpClient client = this.httpClientFactory.CreateClient()) {
                // var result = await client.GetAsync("https://"+this.Request.Host.Value + "/api/servers");
                // IEnumerable<Server> serversList = JsonConvert.DeserializeObject<IEnumerable<Server>>(await result.Content.ReadAsStringAsync());
                foreach (Server server in serversList) {
                    try {
                        var response = await client.GetAsync($"{server.URL}/api/FlightPlan/{id}");
                        var arr = await response.Content.ReadAsStringAsync();
                        flightPlan = JsonConvert.DeserializeObject<FlightPlan>(await response.Content.ReadAsStringAsync());
                        if (flightPlan != null) {
                            flightPlan.Id = id;
                            flightPlansId.Remove(id);
                            return Ok(flightPlan);
                        }
                    }
                    catch (Exception exception) {
                        //todo add logger
                        continue;
                    }
                }
            }
            flightPlansId.Remove(id);
            return NotFound(id);
        }

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan(FlightPlan flightPlan) {
            flightPlan.Id = GenerateFlightId(flightPlan);
            if (FlightPlanExists(flightPlan.Id)) {
                return Conflict(flightPlan);
            }
            //Flight flight = new Flight(flightPlan);
            //var sendRequest = await this.postFlight(flight);
            //if (!sendRequest.IsSuccessStatusCode) {
            //    Debug.WriteLine(sendRequest);
            //    return BadRequest(flight);
            //}

            this.context.FlightPlans.Add(flightPlan);
            await this.context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFlightPlan), new { id = flightPlan.Id }, flightPlan);
        }

        [HttpPost("List")]
        public async Task<ActionResult<IEnumerable<FlightPlan>>> PostFlightPlansList(IEnumerable<FlightPlan> flightPlans) {
            foreach (FlightPlan flightPlan in flightPlans) {
                var actionResult = await PostFlightPlan(flightPlan);
                if (!(actionResult.Result is CreatedAtActionResult)) {
                    return BadRequest(flightPlan);
                }
            }

            return CreatedAtAction(nameof(GetFlightPlans), flightPlans);
        }
        
        private bool FlightPlanExists(string id) {
            return this.context.FlightPlans.Any(e => e.Id == id);
        }

        private async Task<HttpResponseMessage> postFlight(Flight flight) {
            using (HttpClient client = new HttpClient()) {
                var check = this.Request.Host.Value;
                var uri = new Uri("https://" + this.Request.Host.Value + "/api/Flights");
                string contents = JsonConvert.SerializeObject(flight);
                var response = client.PostAsync(uri, new StringContent(contents, Encoding.UTF8, "application/json"));
                var result = await response.ConfigureAwait(false);
                if (!result.IsSuccessStatusCode) {
                    return result;
                }

                return result;
            }
        }

        private static string GenerateFlightId(FlightPlan flightPlan) {
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
