namespace DataAccessLibrary.Models {
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// Api Server date model
    /// </summary>
    public class Server {
        [Key]
        [JsonProperty("ServerId")]
        public string? Id { get; set; }
        [DisplayName("External Servers URL")]
        [Required(ErrorMessage = "A URL is required")]
        [MaxLength(1024)]
        [JsonProperty("ServerURL")]
        [SuppressMessage("Compiler", "CS8618")]
        public string URL { get; set; }

        /// <summary>
        /// Checks for Equality with the specified other based on URL property.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
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
