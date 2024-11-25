using System;
using System.Collections.Generic;

namespace RoadReady.Models
{
    public partial class CarExtra
    {
       public CarExtra()
       {
            ReservationExtra = new HashSet<ReservationExtra>();
            Reservation = new HashSet<Reservation>();
        }

        public int ExtraId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }

        public virtual ICollection<ReservationExtra>? ReservationExtra { get; set; }
        public virtual ICollection<Reservation> ?Reservation { get; set; }
    }
}
