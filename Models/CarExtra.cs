using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RoadReady.Models
{
    public partial class CarExtra
    {
        public CarExtra()
        {
            Reservation = new HashSet<Reservation>(); // Direct collection of reservations
        }

        public int ExtraId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }

        // Directly linked reservations (many-to-many)
        [JsonIgnore]
        public virtual ICollection<Reservation> Reservation { get; set; }
    }
}

