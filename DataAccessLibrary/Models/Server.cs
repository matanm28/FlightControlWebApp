using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models {
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    public class Server {
        [JsonProperty("ServerId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("External Servers URL")]
        [Required(ErrorMessage = "A URL is required")]
        [MaxLength(1024)]
        [JsonProperty("ServerURL")]
        public string URL { get; set; }
    }
}
