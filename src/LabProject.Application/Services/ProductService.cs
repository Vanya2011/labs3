
using LabProject.Application.Interfaces;
using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;

namespace LabProject.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return _productRepository.GetAllAsync();
        }

        public Task<Product?> GetProductByIdAsync(Guid id)
        {
            return _productRepository.GetByIdAsync(id);
        }

        public Task<Product> CreateProductAsync(Product product)
        {
            return _productRepository.AddAsync(product);
        }

        public Task<bool> UpdateProductAsync(Product product)
        {
            return _productRepository.UpdateAsync(product);
        }

        public Task<bool> DeleteProductAsync(Guid id)
        {
            return _productRepository.DeleteAsync(id);
        }

        public Task DeleteAllProductsAsync()
        {
            return _productRepository.DeleteAllAsync();
        }
    }
}
