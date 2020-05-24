using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text.Json.Serialization;
    using DataAccessLibrary.Converters;
    using MathNet.Numerics;
    using MathNet.Numerics.Interpolation;

    public class FlightPlan {
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
        public virtual Location InitialLocation { get; set; }
        public virtual ICollection<Segment> Segments { get; set; }

        [JsonIgnore]
        [NotMapped]
        public DateTime EndTime {
            get {
                TimeSpan totalTimeSpan = TimeSpan.Zero;
                foreach (Segment segment in Segments) {
                    totalTimeSpan += segment.TimeSpan;
                }

                return this.InitialLocation.DateTime.Add(totalTimeSpan);
            }
        }

        public bool IsOngoing(DateTime relativeTo) {
            return relativeTo >= InitialLocation.DateTime && relativeTo <= EndTime;
        }

        public Location GetRelativeLocation(DateTime relativeTo) {
            if (this.IsOngoing(relativeTo)) {
                return this.InterpolateLocation(relativeTo);
            } else if (Segments.Count > 0) {
                return new Location
                           {
                               Id = this.Id,
                               Latitude = this.Segments.Last().Longitude,
                               Longitude = this.Segments.Last().Longitude,
                               DateTime = this.EndTime
                           };
            }

            return this.InitialLocation;
        }

        private Location InterpolateLocation(DateTime relativeTo) {
            IList<double> longitudes = new List<double>();
            IList<double> latitudes = new List<double>();
            IList<double> timeSpans = new List<double>();
            TimeSpan currentTimeSpan = TimeSpan.Zero;
            TimeSpan myTimeSpan = TimeSpan.Zero;
            longitudes.Add(this.InitialLocation.Longitude);
            latitudes.Add(InitialLocation.Latitude);
            timeSpans.Add(currentTimeSpan.TotalSeconds);
            foreach (Segment segment in Segments) {
                longitudes.Add(segment.Longitude);
                latitudes.Add(segment.Latitude);
                currentTimeSpan += segment.TimeSpan;
                timeSpans.Add(currentTimeSpan.TotalSeconds);
            }

            myTimeSpan = relativeTo - this.InitialLocation.DateTime;
            IInterpolation longitudesInterpolation = Interpolate.Linear(timeSpans, longitudes);
            IInterpolation latitudesInterpolation = Interpolate.Linear(timeSpans, latitudes);
            return new Location
                       {
                           Id = this.Id,
                           Longitude = longitudesInterpolation.Interpolate(myTimeSpan.TotalSeconds),
                           Latitude = latitudesInterpolation.Interpolate(myTimeSpan.TotalSeconds),
                           DateTime = relativeTo
                       };
        }

        private Location InterpolateLocation2(DateTime relativeTo) {
            Location currentLocation = this.InitialLocation;
            Segment currentSegment = null;
            foreach (Segment segment in this.Segments) {
                if (currentLocation.DateTime + segment.TimeSpan >= relativeTo) {
                    currentSegment = segment;
                    break;
                }

                currentLocation = new Location
                                      {
                                          Id = currentLocation.Id,
                                          Longitude = segment.Longitude,
                                          Latitude = segment.Latitude,
                                          DateTime = currentLocation.DateTime + segment.TimeSpan
                                      };
            }

            if (currentSegment != null) {
                TimeSpan proportionalTimeSpan = relativeTo - currentLocation.DateTime;
                double linear = proportionalTimeSpan / currentSegment.TimeSpan;
            }

            return null;
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            using (SHA256 hashFunction = SHA256.Create()) {
                double segmentsLatitudeLongitudeSum = 0;
                double segmentsTotalSeconds = 0;
                foreach (Segment segment in Segments) {
                    segmentsLatitudeLongitudeSum += segment.Latitude + segment.Longitude;
                    segmentsTotalSeconds += segment.TimeSpan.TotalSeconds;
                }

                var hashValue = hashFunction.ComputeHash(Encoding.UTF8.GetBytes(this.CompanyName + this.Passengers 
                                                                                + this.InitialLocation.Longitude + this.InitialLocation.Latitude 
                                                                                + this.InitialLocation.DateTime.ToString() + segmentsTotalSeconds 
                                                                                + segmentsLatitudeLongitudeSum));
                return BitConverter.ToInt32(hashValue);
            }
        }

    }
}
