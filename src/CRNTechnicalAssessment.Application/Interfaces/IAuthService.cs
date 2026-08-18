using CRNTechnicalAssessment.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRNTechnicalAssessment.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(
            LoginRequestDto request);

        Task<AuthResponseDto?> RefreshTokenAsync(
            RefreshTokenRequestDto request);
    }
}
