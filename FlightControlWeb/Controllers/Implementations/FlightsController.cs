﻿namespace FlightControlWeb.Controllers.Implementations {

    using DataAccessLibrary.DataAccess.Enums;
    using DataAccessLibrary.DataAccess.Interfaces;
    using DataAccessLibrary.Models;
    using FlightControlWeb.Controllers.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase, IFlightsController {
        private readonly IFlightPlansService flightPlansService;
        private readonly IServerService serverService;
        private readonly IHttpClientFactory httpClientFactory;

        /// <inheritdoc />
        public FlightsController(IFlightPlansService flightPlansService, IServerService serverService, IHttpClientFactory httpClientFactory) {
            this.flightPlansService = flightPlansService;
            this.serverService = serverService;
            this.httpClientFactory = httpClientFactory;
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

            return this.Ok(flights);
        }

        // GET: api/Flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(string id) {
            var flightPlan = await this.flightPlansService.FindAsync(id);
            if (flightPlan == null) {
                return this.NotFound();
            }

            return this.Ok(new Flight(flightPlan));
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(string id) {
            if (!await this.flightPlansService.ExistsAsync(id)) {
                return this.NotFound($"A flight with id:'{id}' doesn't exists on this server");
            }

            var flightPlan = await this.flightPlansService.RemoveAsync(id);
            var flight = new Flight(flightPlan);
            await this.flightPlansService.SaveChangesAsync();

            return Ok(flight);
        }

        // GET: api/Flights?relativeTo=<DateTime>&sync_all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlightsRelativeTo([FromQuery(Name = "relative_to")] DateTime relativeTo) {
            if (relativeTo.Kind != DateTimeKind.Utc) {
                return this.BadRequest($"requset:\"{this.Request}\"\nstatus:\"failed\"\nreason:\"relativeTo format must be yyyy-MM-ddTHH:mm:ssZ (UTC time)\"");
            }

            var flightPlansList = await this.flightPlansService.GetAllAsync(flight => relativeTo >= flight.InitialLocation.DateTime);
            IList<Task<Flight>> flightTasks = new List<Task<Flight>>();
            foreach (FlightPlan flightPlan in flightPlansList) {
                flightTasks.Add(Flight.GetFlightRelativeToTimeAsync(flightPlan, relativeTo));
            }

            List<Flight> flightList = new List<Flight>(await Task.WhenAll(flightTasks));

            if (this.Request != null && this.Request.Query.Keys.Contains("sync_all")) {
                IList<Flight> externalFlights = await this.getFlightsFromExternalServersAsync(relativeTo);
                flightList.AddRange(externalFlights);
            }

            flightList.RemoveAll(flight => flight == null);
            return Ok(flightList);
        }

        private async Task<IList<Flight>> getFlightsFromExternalServersAsync(DateTime relative_to) {
            var servers = await this.serverService.GetAllAsync();
            IList<Task<HttpResponseMessage>> tasksList = new List<Task<HttpResponseMessage>>();
            HttpClient client = this.httpClientFactory.CreateClient(nameof(IServerService));
            foreach (Server server in servers) {
                var uri = new Uri($"{server.URL}/api/Flights?relative_to={relative_to.ToString("yyyy-MM-ddTHH:mm:ssZ")}");
                tasksList.Add(client.GetAsync(uri));
            }
            try {
                Task.WaitAll(tasksList.ToArray());
            } catch (AggregateException e) {
                await Console.Error.WriteLineAsync(e.Message);
            }
            client.Dispose();
            List<Flight> flightsList = await deserializeFlights(tasksList);

            flightsList.ForEach(flight => flight.IsExternal = true);

            return flightsList;
        }

        private static async Task<List<Flight>> deserializeFlights(IList<Task<HttpResponseMessage>> tasksList) {
            List<Flight> flightsList = new List<Flight>();
            foreach (Task<HttpResponseMessage> task in tasksList) {
                if (task.Status == TaskStatus.RanToCompletion) {
                    try {
                        var response = await task;
                        var str = await response.Content.ReadAsStringAsync();
                        IEnumerable<Flight> tempFlights = JsonConvert.DeserializeObject<IEnumerable<Flight>>(await response.Content.ReadAsStringAsync());
                        if (tempFlights != null) {
                            flightsList.AddRange(tempFlights);
                        }
                    } catch (Exception e) {
                        //todo add logger
                        await Console.Error.WriteLineAsync(e.Message);
                    }
                }
            }

            return flightsList;
        }

    }
}