using System;
using System.Collections.Generic;

namespace DataAccessLibrary.Models {
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using MathNet.Numerics;
    using MathNet.Numerics.Interpolation;
    using Newtonsoft.Json;

    public class Flight {
        [Key]
        [Required]
        [JsonProperty("flight_id")]
        [SuppressMessage("Compiler", "CS8618")]
        public string FlightId { get; set; }
        [JsonProperty("company_name")]
        [Required(ErrorMessage = "A Company Name is required")]
        [MaxLength(100)]
        [SuppressMessage("Compiler", "CS8618")]
        public string CompanyName { get; set; }

        [Required]
        [Range(-180.000001, 180, ErrorMessage = "{0} value must be between {1} to {2}")]
        public double Longitude { get; set; }
        [Required]
        [Range(-90.000001, 90, ErrorMessage = "{0} value must be between {1} to {2}")]
        public double Latitude { get; set; }
        [Required]
        [JsonProperty("date_time")]
        public DateTime DateTime { get; set; }

        [Required]
        public int Passengers { get; set; }

        [JsonProperty("is_external")]
        [DefaultValue(false)]
        public bool IsExternal { get; set; }

        [SuppressMessage("Compiler", "CS8618")]
        public Flight() { }

        public Flight(FlightPlan flightPlan, bool isExternal = false) {
            if (flightPlan == null) {
                throw new NullReferenceException();
            }

            this.FlightId = flightPlan.Id ?? string.Empty;
            this.CompanyName = flightPlan.CompanyName;
            this.Passengers = flightPlan.Passengers;
            this.DateTime = flightPlan.InitialLocation.DateTime;
            this.Longitude = flightPlan.InitialLocation.Longitude;
            this.Latitude = flightPlan.InitialLocation.Latitude;
            this.IsExternal = isExternal;
        }

        private Flight(FlightPlan flightPlan, Location currentLocation, bool isExternal = false) {
            this.FlightId = flightPlan.Id ?? string.Empty;
            this.CompanyName = flightPlan.CompanyName;
            this.Passengers = flightPlan.Passengers;
            this.DateTime = currentLocation.DateTime;
            this.Latitude = currentLocation.Latitude;
            this.Longitude = currentLocation.Longitude;
            this.IsExternal = isExternal;
        }

        public static async Task<Flight> GetFlightRelativeToTimeAsync(FlightPlan flightPlan, DateTime relativeTo) {
            if (!flightPlan.IsOngoing(relativeTo)) {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }

            Location location = await InterpolateLocationAsync(relativeTo, flightPlan.InitialLocation, flightPlan.Segments);
            return new Flight(flightPlan, location);
        }

        private static Task<Location> InterpolateLocationAsync(DateTime relativeTo, Location initialLocation, IEnumerable<Segment> segments) {
            IList<double> longitudes = new List<double>();
            IList<double> latitudes = new List<double>();
            IList<double> timeSpans = new List<double>();
            TimeSpan currentTimeSpan = TimeSpan.Zero;
            TimeSpan myTimeSpan = TimeSpan.Zero;
            longitudes.Add(initialLocation.Longitude);
            latitudes.Add(initialLocation.Latitude);
            timeSpans.Add(currentTimeSpan.TotalSeconds);
            foreach (Segment segment in segments) {
                longitudes.Add(segment.Longitude);
                latitudes.Add(segment.Latitude);
                currentTimeSpan += segment.TimeSpan;
                timeSpans.Add(currentTimeSpan.TotalSeconds);
            }

            myTimeSpan = relativeTo - initialLocation.DateTime;
            IInterpolation longitudesInterpolation = Interpolate.Linear(timeSpans, longitudes);
            IInterpolation latitudesInterpolation = Interpolate.Linear(timeSpans, latitudes);
            return Task.FromResult(
                    new Location
                            {
                                    Longitude = longitudesInterpolation.Interpolate(myTimeSpan.TotalSeconds),
                                    Latitude = latitudesInterpolation.Interpolate(myTimeSpan.TotalSeconds),
                                    DateTime = relativeTo
                            });
        }

        protected bool Equals(Flight other) {
            return this.FlightId == other.FlightId && this.CompanyName == other.CompanyName && this.Longitude.Equals(other.Longitude)
                   && this.Latitude.Equals(other.Latitude) && this.DateTime.Equals(other.DateTime) && this.Passengers == other.Passengers
                   && this.IsExternal == other.IsExternal;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != this.GetType()) {
                return false;
            }

            return Equals((Flight)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return HashCode.Combine(this.CompanyName,this.Latitude,this.Longitude,this.DateTime,this.Passengers);
        }
    }
}
