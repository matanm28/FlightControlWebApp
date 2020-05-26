namespace DataAccessLibrary.Models {
    using System;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class Segment {
        [JsonIgnore]
        public int Id { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        [JsonProperty("timespan_seconds")]
        public int TimeSpanSeconds { get; set; }

        [JsonIgnore]
        public TimeSpan TimeSpan {
            get { return new TimeSpan(0,0,0, this.TimeSpanSeconds); }
        }
    }
}
