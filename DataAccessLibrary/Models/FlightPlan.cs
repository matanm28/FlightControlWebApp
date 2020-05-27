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
        
        /// <inheritdoc />
        public override int GetHashCode() {
            return HashCode.Combine(this.Id, this.Passengers, this.CompanyName, this.InitialLocation, this.Segments);
        }

        protected bool Equals(FlightPlan other) {
            return this.Id == other.Id && this.Passengers == other.Passengers && this.CompanyName == other.CompanyName && this.InitialLocation.Equals(other.InitialLocation) && this.Segments.Equals(other.Segments);
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
    }
}
