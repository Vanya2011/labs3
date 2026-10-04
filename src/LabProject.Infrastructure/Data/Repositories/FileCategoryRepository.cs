using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;

namespace LabProject.Infrastructure.Data.Repositories
{
    public class FileCategoryRepository : ICategoryRepository
    {
        private readonly FileContext _context;
        private readonly string _categoriesFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "categories.json");
        private readonly string _productsFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "products.json");

        public FileCategoryRepository(FileContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            var items = _context.Read<Category>(_categoriesFilePath);
            return Task.FromResult<IEnumerable<Category>>(items);
        }

        public Task<Category?> GetByIdAsync(Guid id)
        {
            var items = _context.Read<Category>(_categoriesFilePath);
            var category = items.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(category);
        }

        public Task<Category> AddAsync(Category category)
        {
            var items = _context.Read<Category>(_categoriesFilePath);
            if (category.Id == Guid.Empty)
            {
                category.Id = Guid.NewGuid();
            }

            items.Add(category);
            _context.Write(_categoriesFilePath, items);
            return Task.FromResult(category);
        }

        public Task<bool> UpdateAsync(Category category)
        {
            var items = _context.Read<Category>(_categoriesFilePath);
            var index = items.FindIndex(c => c.Id == category.Id);
            if (index == -1)
            {
                return Task.FromResult(false);
            }

            items[index].Name = category.Name;
            items[index].Description = category.Description;
            _context.Write(_categoriesFilePath, items);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var items = _context.Read<Category>(_categoriesFilePath);
            var category = items.FirstOrDefault(c => c.Id == id);
            if (category is null)
            {
                return Task.FromResult(false);
            }

            items.Remove(category);
            _context.Write(_categoriesFilePath, items);
            return Task.FromResult(true);
        }

        public Task DeleteAllAsync()
        {
            _context.Write(_categoriesFilePath, new List<Category>());
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            var products = _context.Read<Product>(_productsFilePath);
            var filtered = products.Where(p => p.CategoryId == categoryId).ToList();
            return Task.FromResult<IEnumerable<Product>>(filtered);
        }
    }
}
