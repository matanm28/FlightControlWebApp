using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
        using System.ComponentModel.DataAnnotations;
        using System.ComponentModel.DataAnnotations.Schema;
        using System.Text.Json.Serialization;
        using DataAccessLibrary.Converters;

        public class FlightPlan
        {
                [Key]
                [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
                public int Id { get; set; }
                [Required]
                public int Passengers { get; set; }
                [Required(ErrorMessage = "A Company Name is required")]
                [JsonPropertyName("company_name")]
                [MaxLength(100)]
                public string CompanyName { get; set; }
                [Required(ErrorMessage = "A Location is required")]
                [JsonPropertyName("initial_location")]
                public Location InitialLocation { get; set; }
                public virtual ICollection<Segments> Segments { get; set; }
        }
}