﻿using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public int TokenId { get; set; }

        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
