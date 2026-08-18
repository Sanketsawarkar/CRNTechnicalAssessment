using CRNTechnicalAssessment.Application.DTOs;
using CRNTechnicalAssessment.Application.Interfaces;
using CRNTechnicalAssessment.Domain.Entities;
using FluentValidation;

namespace CRNTechnicalAssessment.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<CreateProductDto> _createProductValidator;

        public ProductService(
            IProductRepository productRepository,
            IValidator<CreateProductDto> createProductValidator)
        {
            _productRepository = productRepository;
            _createProductValidator = createProductValidator;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            var result = await _productRepository.GetPagedAsync(
                pageNumber,
                pageSize);

            return result.Items.Select(MapToDto);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            return product is null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto request, string username)
        {
            var validationResult = await _createProductValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var product = new Product
            {
                ProductName = request.ProductName.Trim(),
                CreatedBy = username,
                CreatedOn = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto request, string username)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return null;
            }

            product.ProductName = request.ProductName.Trim();
            product.ModifiedBy = username;
            product.ModifiedOn = DateTime.UtcNow;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return false;
            }

            _productRepository.Delete(product);
            await _productRepository.SaveChangesAsync();

            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CreatedBy = product.CreatedBy,
                CreatedOn = product.CreatedOn,
                ModifiedBy = product.ModifiedBy,
                ModifiedOn = product.ModifiedOn
            };
        }
    }
}
