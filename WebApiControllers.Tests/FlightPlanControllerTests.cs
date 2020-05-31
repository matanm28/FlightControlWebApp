using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiControllers.Tests {
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using Autofac.Extras.Moq;
    using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
    using DataAccessLibrary.DataAccess.Interfaces;
    using DataAccessLibrary.Models;
    using FlightControlWeb.Controllers.Implementations;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class FlightPlanControllerTests {

        [Fact]
        public static async void GetFlightPlans_ShouldReturnFlightPlansList() {
            using (var mock = AutoMock.GetLoose()) {
                mock.Mock<IFlightPlansService>().Setup(x => x.GetAllAsync()).Returns(getSampleFlightPlans());
                var cls = mock.Create<FlightPlanController>();
                var expected = await getSampleFlightPlans();
                var actual = await cls.GetFlightPlans();
                
                mock.Mock<IFlightPlansService>().Verify(x=>x.GetAllAsync(),Times.Once);
                Assert.NotNull(actual);
                Assert.IsType(typeof(ActionResult<IEnumerable<FlightPlan>>), actual);
                Assert.NotNull(actual.Value);
                Assert.Equal(expected.Count,actual.Value.ToList().Count);
                var actualList = actual.Value.ToList();
                for (int i = 0; i < expected.Count; i++) {
                    Assert.Equal(expected[i],actualList[i]);
                }
            }
        }

        [Theory]
        [InlineData("2020-12-25T23:58:21Z")]
        [InlineData("2020-12-26T23:58:21Z")]
        public static async void GetFlightsRelativeToTime_ShouldReturnCorrectFlightPlansList(String dateTimeString) {
            var relativeTo = DateTime.Parse(dateTimeString,null,DateTimeStyles.AdjustToUniversal);
            using (var mock = AutoMock.GetLoose()) {
                mock.Mock<IFlightPlansService>().Setup(x => x.GetAllAsync(flight => relativeTo >= flight.InitialLocation.DateTime))
                    .Returns(getSampleFlightPlans());
                var cls = mock.Create<FlightsController>();
                var expected = await getSampleFlights(relativeTo);
                var actual = (await cls.GetFlightsRelativeTo(relativeTo));

                Assert.NotNull(actual);
                mock.Mock<IFlightPlansService>().Verify(x => x.GetAllAsync(flight => relativeTo >= flight.InitialLocation.DateTime), Times.Once);
                Assert.IsType<ActionResult<IEnumerable<Flight>>>(actual);
                Assert.NotNull(actual.Value);
                var actualList = actual.Value.ToList();
                Assert.Equal(expected.Count, actualList.Count);
                for (int i = 0; i < expected.Count; i++) {
                    Assert.Equal(expected[i], actualList[i]);
                }
            }
        }

        [Theory]
        [InlineData("2020-12-25T23:58:21Z")]
        [InlineData("2020-12-26T23:58:21Z")]
        public static async void GetFlightsRelativeToTime_ShouldNotExceptNonUtcTime(DateTime dateTime) {
            using (var mock = AutoMock.GetLoose()) {
                var cls = mock.Create<FlightsController>();
                var expected = await cls.GetFlightsRelativeTo(dateTime);
                var result = expected.Result as BadRequestResult;
                Assert.IsType(typeof(ActionResult<IEnumerable<Flight>>), expected);
                mock.Mock<IFlightPlansService>().Verify(x=>x.GetAllAsync(),Times.Never);
            }
            
            
            
        }

        public static async void getFlightPlanById_ShouldReturnCorrectFlight() {

        }

        private static Task<List<FlightPlan>> getSampleFlightPlans() {
            List<FlightPlan> output = new List<FlightPlan>() 
            {
                 new FlightPlan()
                 {
                     Id = "FLP-1",
                     Passengers = 248,
                     CompanyName = "First Air",
                     InitialLocation = new Location() 
                     {
                         Id = 1,
                         Latitude = 87.87,
                         Longitude = 72.72,
                         DateTime = DateTime.Parse("2020-12-26T23:56:21Z").ToUniversalTime()
                     },
                     Segments = new List<Segment>()
                     {
                        new Segment()
                        {
                            Id = 1,
                            Latitude = 65.236,
                            Longitude = 37.4239,
                            TimeSpanSeconds = 250
                        },
                        new Segment()
                        {
                            Id = 2,
                            Latitude = 6.236,
                            Longitude = 7.4239,
                            TimeSpanSeconds = 1250
                        },
                        new Segment()
                        { 
                            Id = 3, 
                            Latitude = 3.236, 
                            Longitude = 27.4239, 
                            TimeSpanSeconds = 50
                        }
                     }
                 },
                 new FlightPlan()
                 {
                     Id = "FLP-2",
                     Passengers = 238,
                     CompanyName = "Second Air",
                     InitialLocation = new Location()
                     {
                        Id = 2,
                        Latitude = 87.87,
                        Longitude = 72.72,
                        DateTime = DateTime.Parse("2020-12-25T23:56:21Z").ToUniversalTime()
                     },
                     Segments = new List<Segment>()
                     {
                        new Segment()
                        {
                            Id = 4,
                            Latitude = 56.236,
                            Longitude = 73.49,
                            TimeSpanSeconds = 3250
                        },
                        new Segment() 
                        {
                            Id = 5,
                            Latitude = 26.236,
                            Longitude = 57.4239,
                            TimeSpanSeconds = 450
                        },
                        new Segment()
                        {
                            Id = 6,
                            Latitude = 83.236,
                            Longitude = 62.4239,
                            TimeSpanSeconds = 250
                        }
                     }
                 }
            };
            return Task.FromResult(output);
        }

        private async static Task<List<Flight>> getSampleFlights(DateTime relativeTo) {
            List<Flight> output = new List<Flight>();
            foreach (FlightPlan sampleFlightPlan in (await getSampleFlightPlans()).Where(flight => relativeTo >= flight.InitialLocation.DateTime)) {
                output.Add(await Flight.GetFlightRelativeToTimeAsync(sampleFlightPlan,relativeTo));
            }
            output.RemoveAll(x => x == null);
            return output;
        }
    }
}
