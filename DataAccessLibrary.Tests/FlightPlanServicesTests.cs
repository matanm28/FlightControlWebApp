using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Tests {
    using Autofac.Extras.Moq;
    using Microsoft.EntityFrameworkCore;

    class FlightPlanServicesTests {

        public static void flightPlanService_ShouldReturnValidList() {
            using (var mock = AutoMock.GetLoose()) {
                mock.Mock<DbContext>();
            }
        }
    }
}
