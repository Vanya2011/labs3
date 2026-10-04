using LabProject.Application.Interfaces;
using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;

namespace LabProject.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return _categoryRepository.GetAllAsync();
        }

        public Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return _categoryRepository.GetByIdAsync(id);
        }

        public Task<Category> CreateCategoryAsync(Category category)
        {
            return _categoryRepository.AddAsync(category);
        }

        public Task<bool> UpdateCategoryAsync(Category category)
        {
            return _categoryRepository.UpdateAsync(category);
        }

        public Task<bool> DeleteCategoryAsync(Guid id)
        {
            return _categoryRepository.DeleteAsync(id);
        }

        public Task DeleteAllCategoriesAsync()
        {
            return _categoryRepository.DeleteAllAsync();
        }

        public Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            return _categoryRepository.GetProductsByCategoryIdAsync(categoryId);
        }
    }
}
