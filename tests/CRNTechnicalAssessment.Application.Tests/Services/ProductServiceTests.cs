using CRNTechnicalAssessment.Application.DTOs;
using CRNTechnicalAssessment.Application.Interfaces;
using CRNTechnicalAssessment.Application.Services;
using CRNTechnicalAssessment.Domain.Entities;
using FluentValidation;
using Moq;
using Xunit;

namespace CRNTechnicalAssessment.Application.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IValidator<CreateProductDto>> _validatorMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _validatorMock = new Mock<IValidator<CreateProductDto>>();

            _service = new ProductService(
                _repositoryMock.Object,
                _validatorMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Product_When_Product_Exists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Gaming Laptop",
                CreatedBy = "testuser",
                CreatedOn = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Gaming Laptop", result.ProductName);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Product_Does_Not_Exist()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Product_When_Request_Is_Valid()
        {
            // Arrange
            var request = new CreateProductDto
            {
                ProductName = "Gaming Laptop"
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock
                .Setup(x => x.ValidateAsync(
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(request,"testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Gaming Laptop", result.ProductName);
            Assert.Equal("testuser", result.CreatedBy);

            _repositoryMock.Verify(
                x => x.AddAsync(It.Is<Product>(
                    p => p.ProductName == "Gaming Laptop")),
                Times.Once);

            _repositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Trim_ProductName()
        {
            // Arrange
            var request = new CreateProductDto
            {
                ProductName = "  Laptop  "
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock
                .Setup(x => x.ValidateAsync(
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(request , "testuser");

            // Assert
            Assert.Equal("Laptop", result.ProductName);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Product_Exists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Laptop"
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            _repositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);

            _repositoryMock.Verify(
                x => x.Delete(product),
                Times.Once);

            _repositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_Product_Does_Not_Exist()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);

            _repositoryMock.Verify(
                x => x.Delete(It.IsAny<Product>()),
                Times.Never);
        }
    }
}