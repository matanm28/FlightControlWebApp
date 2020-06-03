namespace DataAccessLibrary.Models {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    public class Segment {
        [JsonIgnore]
        
        public int Id { get; set; }
        [Required]
        [Range(-180.000001, 180, ErrorMessage = "{0} value must be between {1} to {2}")]
        public double Longitude { get; set; }
        [Required]
        [Range(-90.000001, 90, ErrorMessage = "{0} value must be between {1} to {2}")]
        public double Latitude { get; set; }
        [Required]
        [JsonProperty("timespan_seconds")]
        public int TimeSpanSeconds { get; set; }

        [JsonIgnore]
        public TimeSpan TimeSpan {
            get { return new TimeSpan(0, 0, 0, this.TimeSpanSeconds); }
        }

        protected bool Equals(Segment other) {
            return this.Longitude.Equals(other.Longitude) && this.Latitude.Equals(other.Latitude) && this.TimeSpanSeconds == other.TimeSpanSeconds;
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

            return Equals((Segment)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return HashCode.Combine(this.Longitude, this.Latitude, this.TimeSpanSeconds);
        }
    }
}
