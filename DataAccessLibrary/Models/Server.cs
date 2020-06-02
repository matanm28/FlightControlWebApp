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

        protected bool Equals(Server other) {
            return this.URL == other.URL;
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

            return Equals((Server)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.URL.GetHashCode();
        }
    }
}
