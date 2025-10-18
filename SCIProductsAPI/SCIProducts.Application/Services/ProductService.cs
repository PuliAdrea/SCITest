using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCIProducts.Domain.Entities;
using SCIProducts.Application.Services.Interfaces;
using SCIProducts.Infrastructure.Repositories.Interfaces;

namespace SCIProducts.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productRepository.GetAll();
        }

        public async Task<Product?> GetById(int id)
        {
            return await _productRepository.GetById(id);
        }

        public async Task<Product?> Add(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be empty.");

            if (product.Price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            return await _productRepository.Add(product);
        }

        public async Task<Product?> Update(int id, Product product)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid product ID.");

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be empty.");

            if (product.Price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            return await _productRepository.Update(id, product);
        }

        public async Task<bool> Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid product ID.");

            return await _productRepository.Delete(id);
        }
    }
}