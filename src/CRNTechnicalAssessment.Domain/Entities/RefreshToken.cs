using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRNTechnicalAssessment.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRevoked { get; set; }
    }
}
