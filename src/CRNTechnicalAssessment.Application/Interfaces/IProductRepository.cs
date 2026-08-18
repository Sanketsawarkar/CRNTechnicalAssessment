using CRNTechnicalAssessment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRNTechnicalAssessment.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize);

        Task<Product?> GetByIdAsync(int id);

        Task AddAsync(Product product);

        void Update(Product product);

        void Delete(Product product);

        Task<bool> ExistsAsync(int id);

        Task SaveChangesAsync();
    }
}
