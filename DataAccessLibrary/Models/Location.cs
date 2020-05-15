namespace DataAccessLibrary.Models
{
        using System;
        using System.ComponentModel.DataAnnotations;
        using System.ComponentModel.DataAnnotations.Schema;
        using System.Text.Json.Serialization;

        public class Location
        {
                [Key]
                [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
                public int Id { get; set; }
                [Required]

                // todo add range
                public double Longitude { get; set; }
                [Required]

                // todo add range
                
                public double Latitude { get; set; }
                [Required]
                [JsonPropertyName("date_time")]
                public DateTime DateTime { get; set; }
        }
}