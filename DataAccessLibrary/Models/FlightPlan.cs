using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models {
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using DataAccessLibrary.Converters;
    using MathNet.Numerics;
    using MathNet.Numerics.Interpolation;
    using Newtonsoft.Json;

    
    public class FlightPlan {
        [Key]
        [JsonIgnore]
        public string? Id { get; set; }
        [Required(ErrorMessage = "Number of passengers is required")]
        public int Passengers { get; set; }
        [Required(ErrorMessage = "A Company Name is required")]
        [JsonProperty("company_name")]
        [MaxLength(100)]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "A Location is required")]
        [JsonProperty("initial_location")]
        public Location InitialLocation { get; set; }
        public ICollection<Segment> Segments { get; set; }

        [JsonIgnore]
        [NotMapped]
        public DateTime EndTime {
            get {
                TimeSpan totalTimeSpan = TimeSpan.Zero;
                foreach (Segment segment in Segments) {
                    totalTimeSpan += segment.TimeSpan;
                }

                return this.InitialLocation.DateTime + totalTimeSpan;
            }
        }

        public bool IsOngoing(DateTime relativeTo) {
            DateTime start = this.InitialLocation.DateTime;
            DateTime end = this.EndTime;
            return relativeTo >= InitialLocation.DateTime && relativeTo <= EndTime;
        }

        public async Task<Flight?> GetFlightRelativeToTimeAsync(DateTime relativeTo) {
            if (!this.IsOngoing(relativeTo)) {
                return null;
            }
            Location location = await this.InterpolateLocationAsync(relativeTo);
            Flight flight = new Flight()
                                {
                                    FlightId = this.Id,
                                    CompanyName = this.CompanyName,
                                    DateTime = location.DateTime,
                                    Longitude = location.Longitude,
                                    Latitude = location.Latitude,
                                    Passengers = this.Passengers,
                                };
            return flight;
        }


        private Location GetRelativeLocation(DateTime relativeTo) {
            if (this.IsOngoing(relativeTo)) {
                return this.InterpolateLocation(relativeTo);
            } else if (Segments.Count > 0) {
                return new Location
                           {
                               Latitude = this.Segments.Last().Longitude,
                               Longitude = this.Segments.Last().Longitude,
                               DateTime = this.EndTime
                           };
            }

            return this.InitialLocation;
        }

        private Location InterpolateLocation(DateTime relativeTo) {
            IList<double> longitudes = new List<double>();
            IList<double> latitudes = new List<double>();
            IList<double> timeSpans = new List<double>();
            TimeSpan currentTimeSpan = TimeSpan.Zero;
            TimeSpan myTimeSpan = TimeSpan.Zero;
            longitudes.Add(this.InitialLocation.Longitude);
            latitudes.Add(InitialLocation.Latitude);
            timeSpans.Add(currentTimeSpan.TotalSeconds);
            foreach (Segment segment in Segments) {
                longitudes.Add(segment.Longitude);
                latitudes.Add(segment.Latitude);
                currentTimeSpan += segment.TimeSpan;
                timeSpans.Add(currentTimeSpan.TotalSeconds);
            }

            myTimeSpan = relativeTo - this.InitialLocation.DateTime;
            IInterpolation longitudesInterpolation = Interpolate.Linear(timeSpans, longitudes);
            IInterpolation latitudesInterpolation = Interpolate.Linear(timeSpans, latitudes);
            return new Location
                       {
                           Longitude = longitudesInterpolation.Interpolate(myTimeSpan.TotalSeconds),
                           Latitude = latitudesInterpolation.Interpolate(myTimeSpan.TotalSeconds),
                           DateTime = relativeTo
                       };
        }

        private Task<Location> InterpolateLocationAsync(DateTime relativeTo) {
            IList<double> longitudes = new List<double>();
            IList<double> latitudes = new List<double>();
            IList<double> timeSpans = new List<double>();
            TimeSpan currentTimeSpan = TimeSpan.Zero;
            TimeSpan myTimeSpan = TimeSpan.Zero;
            longitudes.Add(this.InitialLocation.Longitude);
            latitudes.Add(InitialLocation.Latitude);
            timeSpans.Add(currentTimeSpan.TotalSeconds);
            foreach (Segment segment in Segments) {
                longitudes.Add(segment.Longitude);
                latitudes.Add(segment.Latitude);
                currentTimeSpan += segment.TimeSpan;
                timeSpans.Add(currentTimeSpan.TotalSeconds);
            }
            myTimeSpan = relativeTo - this.InitialLocation.DateTime;
            IInterpolation longitudesInterpolation = Interpolate.Linear(timeSpans, longitudes);
            IInterpolation latitudesInterpolation = Interpolate.Linear(timeSpans, latitudes);
            return Task.FromResult(new Location {
                                                        Longitude = longitudesInterpolation.Interpolate(myTimeSpan.TotalSeconds),
                                                        Latitude = latitudesInterpolation.Interpolate(myTimeSpan.TotalSeconds),
                                                        DateTime = relativeTo
                                                    });
        }


        /// <inheritdoc />
        public override int GetHashCode() {
            using (SHA256 hashFunction = SHA256.Create()) {
                double segmentsLatitudeLongitudeSum = 0;
                double segmentsTotalSeconds = 0;
                foreach (Segment segment in Segments) {
                    segmentsLatitudeLongitudeSum += segment.Latitude + segment.Longitude;
                    segmentsTotalSeconds += segment.TimeSpan.TotalSeconds;
                }

                var hashValue = hashFunction.ComputeHash(Encoding.UTF8.GetBytes(this.CompanyName + this.Passengers 
                                                                                + this.InitialLocation.Longitude + this.InitialLocation.Latitude 
                                                                                + this.InitialLocation.DateTime.ToString() + segmentsTotalSeconds 
                                                                                + segmentsLatitudeLongitudeSum));
                return BitConverter.ToInt32(hashValue);
            }
        }

    }
}
