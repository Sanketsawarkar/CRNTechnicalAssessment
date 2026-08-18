using CRNTechnicalAssessment.Application.DTOs;
using CRNTechnicalAssessment.Application.Interfaces;
using CRNTechnicalAssessment.Domain.Entities;
using CRNTechnicalAssessment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRNTechnicalAssessment.Infrastructure.Identity;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;

    public UserService(
        ApplicationDbContext context,
        IPasswordHasherService passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponseDto> CreateUserAsync(
        CreateUserRequestDto request)
    {
        var usernameExists = await _context.Users
            .AnyAsync(x => x.Username == request.Username);

        if (usernameExists)
        {
            throw new InvalidOperationException(
                "Username already exists.");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = _passwordHasher.HashPassword(
                request.Password),
            Role = request.Role,
            IsActive = true
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            IsActive = user.IsActive
        };
    }
}