using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using DataAccessLibrary;
using System.Linq;
using Newtonsoft.Json;

namespace DataAccessLibrary.Tests {
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using DataAccessLibrary.Models;

    public class FlightPlanTests {

        [Fact]
        public void FlightPlan_ShouldReturnFlightAccordingly() {
            // Arrange
            var expected = GetTestFlight();
            var flightPlan = JsonConvert.DeserializeObject<FlightPlan>(File.ReadAllText(@"TestFlightPlan.json"));
            // Act
            var actual = new Flight(flightPlan);


            // Assert
            Assert.Equal(actual,expected);

        }
        
        [Fact]
        public void FlightPlan_ShouldThrowException() {
            // Arrange
            FlightPlan nullFlightPlan = null;

            // Act
            Action action = () => new Flight(nullFlightPlan);

            // Assert
            Assert.Throws<NullReferenceException>(action);

        }

        [Fact]
        public void FlightPlan_EqualsEdgeCasesShouldWork() {
            // Arrange
            

            // Act


            // Assert



        }

        private static Flight GetTestFlight() {
            return new Flight {
                                              FlightId = "",
                                              DateTime = DateTime.Parse("2020-05-26T18:08:33Z").ToUniversalTime(),
                                              CompanyName = "Omatom Fly",
                                              Longitude = -93.24,
                                              Latitude = 17.24,
                                              Passengers = 248,
                                              IsExternal = false
                                      };
        }

    }
}
