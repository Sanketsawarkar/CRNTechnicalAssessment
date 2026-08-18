using CRNTechnicalAssessment.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRNTechnicalAssessment.API.Tests
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing ApplicationDbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add an in-memory database for tests
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("CRNTestDatabase");
                });

                // Build service provider
                var serviceProvider =
                    services.BuildServiceProvider();

                // Create the test database
                using var scope =
                    serviceProvider.CreateScope();

                var dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                dbContext.Database.EnsureCreated();

                if (!dbContext.Products.Any())
                {
                    dbContext.Products.Add(new Domain.Entities.Product
                    {
                        Id = 1,
                        ProductName = "Test Product",
                        CreatedBy = "test",
                        CreatedOn = DateTime.UtcNow
                    });

                    dbContext.SaveChanges();
                }
            });
        }
    }
}