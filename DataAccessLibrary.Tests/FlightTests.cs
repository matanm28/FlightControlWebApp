using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Tests {
    using System.Linq;
    using DataAccessLibrary.Models;
    using Xunit;

    public static class FlightTests {
        [Fact]
        public static void Flight_HashCodeShouldBeAffectedByPointerAddress() {
            // Arrange
            Random rand = new Random();
            Flight flight = new Flight()
                                    {
                                            DateTime = DateTime.Today,
                                            CompanyName = rand.RandomString(7),
                                            Latitude = rand.Next(-89, 90) + rand.NextDouble(),
                                            Longitude = rand.Next(-179, 180) + rand.NextDouble(),
                                            Passengers = rand.Next(500)
                                    };

            // Act

            // Assert
        }

        private static string RandomString(this Random random,int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
