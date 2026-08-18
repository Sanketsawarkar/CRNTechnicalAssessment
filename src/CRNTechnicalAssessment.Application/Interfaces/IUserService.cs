using CRNTechnicalAssessment.Application.DTOs;

namespace CRNTechnicalAssessment.Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request);
}