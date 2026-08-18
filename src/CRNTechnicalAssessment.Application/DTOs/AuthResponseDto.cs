using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRNTechnicalAssessment.Application.DTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime AccessTokenExpiresAt { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
