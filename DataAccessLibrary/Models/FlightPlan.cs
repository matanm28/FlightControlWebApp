using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using Newtonsoft.Json;

    public class FlightPlan {
        [Key]
        [JsonIgnore]
        public string Id { get; set; }
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

        /// <inheritdoc />
        public override int GetHashCode() {
            using (SHA256 hashFunction = SHA256.Create()) {
                double segmentsLatitudeLongitudeSum = 0;
                double segmentsTotalSeconds = 0;
                foreach (Segment segment in Segments) {
                    segmentsLatitudeLongitudeSum += segment.Latitude + segment.Longitude;
                    segmentsTotalSeconds += segment.TimeSpan.TotalSeconds;
                }

                var hashValue = hashFunction.ComputeHash(
                        Encoding.UTF8.GetBytes(
                                this.CompanyName + this.Passengers + this.InitialLocation.Longitude + this.InitialLocation.Latitude
                                + this.InitialLocation.DateTime.ToString() + segmentsTotalSeconds + segmentsLatitudeLongitudeSum));
                return BitConverter.ToInt32(hashValue);
            }
        }

        protected bool Equals(FlightPlan other) {
            if (this.Segments.Count == other.Segments.Count) {
                for (int i = 0; i < this.Segments.Count; i++) {
                    if (!this.Segments.ElementAt(i).Equals(other.Segments.ElementAt(i))) {
                        return false;
                    }
                }

                return this.Passengers == other.Passengers && this.CompanyName == other.CompanyName && this.InitialLocation.Equals(other.InitialLocation);
            }

            return false;
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

            return Equals((FlightPlan)obj);
        }

        public static bool IsValidFlightPlanModel(FlightPlan flightPlan) {
            if (flightPlan == null) {
                return false;
            }
            foreach (PropertyInfo propertyInfo in flightPlan.GetType().GetProperties()) {
                if (propertyInfo.GetValue(flightPlan) == null) {
                    return false;
                }
            }

            return true;
        }
    }
}
