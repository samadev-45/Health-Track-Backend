using System;

namespace Health.Domain.Entities
{
    public class OtpVerification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OtpCode { get; set; } 
        public string Purpose { get; set; } = null!;
        public DateTime Expiry { get; set; }
        public bool Used { get; set; } = false;

        public User User { get; set; } = null!;
    }
}
