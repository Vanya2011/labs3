using LabProject.Application.DTOs.Product;
using LabProject.Domain.Models;

namespace LabProject.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto?> GetProductByIdAsync(Guid id);
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
        Task<bool> UpdateProductAsync(Guid id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(Guid id);
        Task DeleteAllProductsAsync();
    }
}
