using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
        using System.ComponentModel;
        using System.ComponentModel.DataAnnotations;
        using System.Text.Json.Serialization;
        using DataAccessLibrary.Converters;

        public class Flight
        {
                [Key]
                [Required]
                public string FlightId { get; set; }
                [Required(ErrorMessage = "A Company Name is required")]
                [JsonPropertyName("company_name")]
                [MaxLength(100)]
                public string CompanyName { get; set; }

                [Required]
                public double Longitude { get; set; }
                [Required]
                public double Latitude { get; set; }
                [Required]
                [JsonPropertyName("date_time")]
                public DateTime DateTime { get; set; }

                [Required]
                public int Passengers { get; set; }

                [JsonPropertyName("is_external")]
                [DefaultValue(false)]
                public bool IsExternal { get; set; }
        }
}