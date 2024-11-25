using System;
using System.Collections.Generic;

namespace RoadReady.Models
{
    public partial class PasswordReset
    {
        public int ResetId { get; set; }
        public int? UserId { get; set; }
        public string? Token { get; set; }
        public DateTime? RequestTime { get; set; }
        public DateTime? ResetTime { get; set; }

        public virtual User? User { get; set; }
    }
}
