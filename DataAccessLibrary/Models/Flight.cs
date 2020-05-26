using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models {
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using DataAccessLibrary.Converters;
    using Newtonsoft.Json;

    public class Flight {
        [Key]
        [Required]
        [JsonProperty("flight_id")]
        public string FlightId { get; set; }
        [JsonProperty("company_name")]
        [Required(ErrorMessage = "A Company Name is required")]
        [MaxLength(100)]
        public string CompanyName { get; set; }

        [Required]
        [Range(-180.000001, 180)]
        public double Longitude { get; set; }
        [Required]
        [Range(-90.000001, 90)]
        public double Latitude { get; set; }
        [Required]
        [JsonProperty("date_time")]
        public DateTime DateTime { get; set; }

        [Required]
        public int Passengers { get; set; }

        [JsonProperty("is_external")]
        [DefaultValue(false)]
        public bool IsExternal { get; set; }
    }
}
