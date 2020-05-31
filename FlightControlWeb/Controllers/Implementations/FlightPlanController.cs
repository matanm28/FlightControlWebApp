﻿namespace FlightControlWeb.Controllers.Implementations {
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Castle.Core.Internal;
    using DataAccessLibrary.DataAccess.Interfaces;
    using DataAccessLibrary.Models;
    using FlightControlWeb.Controllers.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase, IFlightPlanController {
        private static ISet<string> flightPlansId = new HashSet<string>();
        private const int ModNumber = 100000;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ServersController serversController;
        private readonly IFlightPlansService flightPlansService;

        /// <inheritdoc />
        public FlightPlanController(IFlightPlansService flightPlansService, IHttpClientFactory httpClientFactory, ServersController serversController) {
            this.flightPlansService = flightPlansService;
            this.serversController = serversController;
            this.httpClientFactory = httpClientFactory;
        }

        // GET: api/FlightPlans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightPlans() {
            return await this.flightPlansService.GetAllAsync();
        }

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id) {
            if (flightPlansId.Contains(id)) {
                return this.NotFound(id);
            }

            var flightPlan = (await this.flightPlansService.FindAsync(id));
            if (flightPlan != null) {
                return this.Ok(flightPlan);
            }

            var serversList = (await this.serversController.GetApiServer()).Value;
            if (serversList.IsNullOrEmpty()) {
                return this.NotFound(id);
            }

            flightPlansId.Add(id);
            using (HttpClient client = this.httpClientFactory.CreateClient(nameof(IServerService))) {
                // var result = await client.GetAsync("https://"+this.Request.Host.Value + "/api/servers");
                // IEnumerable<Servers> serversList = JsonConvert.DeserializeObject<IEnumerable<Servers>>(await result.Content.ReadAsStringAsync());
                foreach (Server server in serversList) {
                    try {
                        var response = await client.GetAsync($"{server.URL}/api/FlightPlan/{id}");
                        var arr = await response.Content.ReadAsStringAsync();
                        flightPlan = JsonConvert.DeserializeObject<FlightPlan>(await response.Content.ReadAsStringAsync());
                        if (flightPlan != null) {
                            flightPlan.Id = id;
                            flightPlansId.Remove(id);
                            return this.Ok(flightPlan);
                        }
                    }
                    catch (Exception exception) {
                        // todo add logger
                        continue;
                    }
                }
            }

            flightPlansId.Remove(id);
            return this.NotFound(id);
        }

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan(FlightPlan flightPlan) {
            flightPlan.Id = GenerateFlightId(flightPlan);
            if (await this.flightPlansService.ExistsAsync(flightPlan.Id)) {
                return this.Conflict(flightPlan);
            }

            await this.flightPlansService.AddAsync(flightPlan);
            await this.flightPlansService.SaveChangesAsync();

            return this.CreatedAtAction(nameof(this.GetFlightPlan), new { id = flightPlan.Id }, flightPlan);
        }

        [HttpPost("List")]
        public async Task<ActionResult<IEnumerable<FlightPlan>>> PostFlightPlansList(IEnumerable<FlightPlan> flightPlans) {
            List<FlightPlan> acceptedFlightPlans = new List<FlightPlan>();
            foreach (FlightPlan flightPlan in flightPlans) {
                flightPlan.Id = GenerateFlightId(flightPlan);
                if (!await this.flightPlansService.ExistsAsync(flightPlan.Id)) {
                    await this.flightPlansService.AddAsync(flightPlan);
                    acceptedFlightPlans.Add(flightPlan);
                }
            }

            await this.flightPlansService.SaveChangesAsync();
            return this.CreatedAtAction(nameof(this.GetFlightPlans), acceptedFlightPlans);
        }

        private static string GenerateFlightId(FlightPlan flightPlan) {
            StringBuilder sb = new StringBuilder();
            int mod = ModNumber;
            string trimmedCompanyName = flightPlan.CompanyName.Replace(" ", string.Empty);
            int length = trimmedCompanyName.Length;
            if (length < 5) {
                for (int i = 0; i < 5 - length; i++) {
                    mod *= 10;
                }

                sb.Append(trimmedCompanyName.Substring(0, length));
            } else {
                sb.Append(trimmedCompanyName.Substring(0, 5));
            }

            // var key = BitConverter.ToUInt64(hashedValue) % mod;
            var key = Math.Abs(flightPlan.GetHashCode()) % mod;
            return sb.Append($"-{key}").ToString().ToUpper();
        }
    }
}
