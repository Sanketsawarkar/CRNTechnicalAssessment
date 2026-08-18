using CRNTechnicalAssessment.Application.DTOs;
using CRNTechnicalAssessment.Application.Interfaces;
using CRNTechnicalAssessment.Application.Settings;
using CRNTechnicalAssessment.Domain.Entities;
using CRNTechnicalAssessment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace CRNTechnicalAssessment.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IPasswordHasherService _passwordHasher;

    public AuthService(
        ApplicationDbContext context,
        IOptions<JwtSettings> jwtOptions,
        JwtTokenService jwtTokenService,
        IPasswordHasherService passwordHasher)
    {
        _context = context;
        _jwtSettings = jwtOptions.Value;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto?> LoginAsync(
        LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Username == request.Username &&
                x.IsActive);

        if (user == null)
        {
            return null;
        }

        var passwordValid = _passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            return null;
        }

        var accessTokenExpiresAt =
            DateTime.UtcNow.AddMinutes(
                _jwtSettings.AccessTokenExpirationMinutes);

        var accessToken = _jwtTokenService.GenerateAccessToken(
            user.Username,
            user.Role,
            accessTokenExpiresAt);

        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Username = user.Username,
            Token = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(
        RefreshTokenRequestDto request)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.Token == request.RefreshToken);

        if (storedToken == null ||
            storedToken.IsRevoked ||
            storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Username == storedToken.Username &&
                x.IsActive);

        if (user == null)
        {
            return null;
        }

        var accessTokenExpiresAt =
            DateTime.UtcNow.AddMinutes(
                _jwtSettings.AccessTokenExpirationMinutes);

        var accessToken = _jwtTokenService.GenerateAccessToken(
            user.Username,
            user.Role,
            accessTokenExpiresAt);

        // Revoke old refresh token.
        storedToken.IsRevoked = true;

        // Generate replacement refresh token.
        var newRefreshToken = GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            Username = user.Username,
            Token = newRefreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            Username = user.Username,
            Role = user.Role
        };
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }
}