using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessLibrary;
using Newtonsoft.Json;
using Xunit;

namespace DataAccessLibrary.Tests {
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using DataAccessLibrary.Models;
    using MathNet.Numerics.Random;

    public static class FlightPlanTests {
        [Fact]
        public static void FlightPlan_ShouldReturnFlightAccordingly() {
            // Arrange
            var expected = getTestFlight();
            var flightPlan = JsonConvert.DeserializeObject<FlightPlan>(File.ReadAllText(@"TestFlightPlan.json"));

            // Act
            var actual = new Flight(flightPlan);

            // Assert
            Assert.Equal(actual, expected);
        }

        [Fact]
        public static void FlightPlan_ShouldThrowException() {
            // Arrange
            FlightPlan nullFlightPlan = null;

            // Act
            Action action = () => new Flight(nullFlightPlan);

            // Assert
            Assert.Throws<NullReferenceException>(action);
        }

        [Fact]
        public static void FlightPlan_EqualsEdgeCasesShouldWork() {
            // Arrange
            FlightPlan flightPlan = getTestFlightPlan();
            FlightPlan selfReferenceFlightPlan = flightPlan;
            FlightPlan nullFlightPlan = null;
            object notSameObjectFlightPlan = new Flight();
            object sameFlightDifferentPointerAddress = new FlightPlan()
                                                               {
                                                                       CompanyName = flightPlan.CompanyName,
                                                                       Passengers = flightPlan.Passengers,
                                                                       InitialLocation =
                                                                               new Location()
                                                                                       {
                                                                                               Latitude = flightPlan.InitialLocation.Latitude,
                                                                                               Longitude = flightPlan.InitialLocation.Longitude,
                                                                                               DateTime = flightPlan.InitialLocation.DateTime
                                                                                       },
                                                                       Segments = new List<Segment>(flightPlan.Segments)
                                                               };

            // Act

            // Assert
            Assert.Equal(flightPlan, selfReferenceFlightPlan);
            Assert.NotEqual(flightPlan, nullFlightPlan);
            Assert.NotEqual(flightPlan, notSameObjectFlightPlan);
            Assert.Equal(flightPlan,sameFlightDifferentPointerAddress);
        }

        [Fact]
        public static void FlightPlan_HashCodeShouldBeAffectedByPointerAddress() {
            // Arrange
            FlightPlan flightPlan = getTestFlightPlan();
            FlightPlan secondFlightPlan = new FlightPlan()
                                                  {
                                                          CompanyName = flightPlan.CompanyName,
                                                          Passengers = flightPlan.Passengers,
                                                          InitialLocation = new Location()
                                                                                    {
                                                                                            Latitude = flightPlan.InitialLocation.Latitude,
                                                                                            Longitude = flightPlan.InitialLocation.Longitude,
                                                                                            DateTime = flightPlan.InitialLocation.DateTime
                                                                                    },
                                                          Segments = new List<Segment>(flightPlan.Segments)
                                                  };

            // Act
            int expected = flightPlan.GetHashCode();
            int actual = secondFlightPlan.GetHashCode();

            // Assert
            Assert.Equal(expected, actual);
        }

        private static string RandomString(this Random random, int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static IList<Segment> getRandomSegments(this Random random) {
            IList<Segment> segments = new List<Segment>();
            while (random.NextBoolean()) {
                segments.Add(
                        new Segment()
                                {
                                        Latitude = random.Next(-89, 90) + random.NextDouble(),
                                        Longitude = random.Next(-179, 180) + random.NextDouble(),
                                        TimeSpanSeconds = random.Next(500)
                                });
            }

            return segments;
        }

        private static Flight getTestFlight() {
            return new Flight
                           {
                                   FlightId = string.Empty,
                                   DateTime = DateTime.Parse("2020-05-26T18:08:33Z").ToUniversalTime(),
                                   CompanyName = "Omatom Fly",
                                   Longitude = -93.24,
                                   Latitude = 17.24,
                                   Passengers = 248,
                                   IsExternal = false
                           };
        }

        private static FlightPlan getTestFlightPlan() {
            Random rand = new Random();
            FlightPlan flightPlan = new FlightPlan()
                                            {
                                                    CompanyName = rand.RandomString(7),
                                                    Passengers = rand.Next(500),
                                                    InitialLocation =
                                                            new Location()
                                                                    {
                                                                            Latitude = rand.Next(-89, 90) + rand.NextDouble(),
                                                                            Longitude = rand.Next(-179, 180) + rand.NextDouble(),
                                                                            DateTime = DateTime.Today
                                                                    },
                                                    Segments = rand.getRandomSegments()
                                            };
            return flightPlan;
        }
    }
}
