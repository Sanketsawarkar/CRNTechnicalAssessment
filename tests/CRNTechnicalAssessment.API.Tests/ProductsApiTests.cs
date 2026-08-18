using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace CRNTechnicalAssessment.API.Tests
{
    public class ProductsApiTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductsApiTests(
               CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProducts_Should_Return_Unauthorized_When_No_Token()
        {
            // Act
            var response = await _client.GetAsync(
                "/api/v1/Products?pageNumber=1&pageSize=10");

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task GetProducts_Should_Return_Ok_When_Valid_Admin_Token()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var jwtKey = configuration["Jwt:Key"];

            var jwtIssuer = configuration["Jwt:Issuer"];

            var jwtAudience = configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT Key is not configured.");
            }

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, "admin"),
        new Claim(ClaimTypes.Name, "admin"),
        new Claim(ClaimTypes.Role, "Admin"),
        new Claim(
            JwtRegisteredClaimNames.Jti,
            Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    tokenString);

            // Act
            var response = await _client.GetAsync(
                "/api/v1/Products?pageNumber=1&pageSize=10");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_Should_Return_Forbidden_When_User_Is_Not_Admin()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
             new Claim(ClaimTypes.Name, "testuser"),
             new Claim(ClaimTypes.Role, "User"),
             new Claim(
             JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    tokenString);

            var request = new
            {
                productName = "Test Product"
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                "/api/v1/Products",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_Should_Return_Created_When_Admin()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub, "admin"),
             new Claim(ClaimTypes.Name, "admin"),
             new Claim(ClaimTypes.Role, "Admin"),
             new Claim(
             JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    tokenString);

            var request = new
            {
                productName = "Integration Test Product"
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                "/api/v1/Products",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_Should_Return_BadRequest_When_ProductName_Is_Empty()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub, "admin"),
             new Claim(ClaimTypes.Name, "admin"),
             new Claim(ClaimTypes.Role, "Admin"),
             new Claim(
            JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
         };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    tokenString);

            var request = new
            {
                productName = ""
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                "/api/v1/Products",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);
        }

        [Fact]
        public async Task GetProductById_Should_Return_Ok_When_Product_Exists()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub, "admin"),
             new Claim(ClaimTypes.Name, "admin"),
             new Claim(ClaimTypes.Role, "Admin"),
             new Claim(
            JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    tokenString);

            // Act
            var response = await _client.GetAsync("/api/v1/Products/1");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }


        [Fact]
        public async Task GetProductById_Should_Return_NotFound_When_Product_Does_Not_Exist()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub, "admin"),
             new Claim(ClaimTypes.Name, "admin"),
             new Claim(ClaimTypes.Role, "Admin"),
             new Claim(
            JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
         };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    tokenString);

            // Act
            var response = await _client.GetAsync(
                "/api/v1/Products/999999");

            // Assert
            Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_Should_Return_Forbidden_When_User_Is_Not_Admin()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
             new Claim(ClaimTypes.Name, "testuser"),
             new Claim(ClaimTypes.Role, "User"),
             new Claim(
            JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    tokenString);

            // Act
            var response = await _client.DeleteAsync(
                "/api/v1/Products/1");

            // Assert
            Assert.Equal(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_Should_Return_NoContent_When_Admin()
        {
            // Arrange
            SetAuthorization("admin", "Admin");

            // Act
            var response = await _client.DeleteAsync(
                "/api/v1/Products/1");

            // Assert
            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);
        }

        private void SetAuthorization(
            string username,
            string role)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT Key is not configured.");
            }

            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    username),

                new Claim(
                    ClaimTypes.Name,
                    username),

                new Claim(
                    ClaimTypes.Role,
                    role),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    tokenString);
        }
    }
}