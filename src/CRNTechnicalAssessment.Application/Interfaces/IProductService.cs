using CRNTechnicalAssessment.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRNTechnicalAssessment.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(int pageNumber,int pageSize);

        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto> CreateAsync(CreateProductDto request, string username);

        Task<ProductDto?> UpdateAsync(int id,UpdateProductDto request, string username);

        Task<bool> DeleteAsync(int id);
    }
}
