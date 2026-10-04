using LabProject.Domain.Models;

namespace LabProject.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category> AddAsync(Category category);
        Task<bool> UpdateAsync(Category category);
        Task<bool> DeleteAsync(Guid id);
        Task DeleteAllAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
    }
}
