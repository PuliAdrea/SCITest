using System.Collections.Generic;
using SCIProducts.Domain.Entities;

namespace SCIProducts.Infrastructure.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product?> GetById(int id);
        Task<Product?> Add(Product product);
        Task<Product?> Update(int id, Product product);
        Task<bool> Delete(int id);
    }
}
