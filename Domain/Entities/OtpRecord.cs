using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class OtpRecord
    {
        public int OtpId { get; set; } 
        public int UserId { get; set; }
        public string Code { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
