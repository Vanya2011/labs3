
using LabProject.Domain.Models;

namespace LabProject.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(Guid id);
        Task<Category> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task DeleteAllCategoriesAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
    }
}
