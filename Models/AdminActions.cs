using System;
using System.Collections.Generic;

namespace RoadReady.Models
{
    public partial class AdminActions
    {
        public int ActionId { get; set; }
        public int? AdminId { get; set; }
        public string? ActionType { get; set; }
        public string? ActionDescription { get; set; }
        public DateTime? ActionDate { get; set; }

        public virtual User? Admin { get; set; }
    }
}
