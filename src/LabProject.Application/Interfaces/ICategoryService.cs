
using LabProject.Application.DTOs.Category;
using LabProject.Application.DTOs.Product;
using LabProject.Domain.Models;

namespace LabProject.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id);
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task DeleteAllCategoriesAsync();
        Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryIdAsync(Guid categoryId);
    }
}
