﻿namespace DataAccessLibrary.Models
{
        using System;
        using System.ComponentModel.DataAnnotations;
        using System.ComponentModel.DataAnnotations.Schema;
        using Newtonsoft.Json;

        public class Location
        {
                [Key]
                [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
                [JsonIgnore]
                
                public int Id { get; set; }
                [Required]
                [Range(-180.000001, 180)]

                public double Longitude { get; set; }
                [Required]
                [Range(-90.000001, 90)]
                public double Latitude { get; set; }
                [Required]
                [JsonProperty("date_time")]
                public DateTime DateTime { get; set; }

        }
}