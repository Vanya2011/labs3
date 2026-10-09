using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LabProject.Infrastructure.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var existing = await _context.Categories.FindAsync(category.Id);
            if (existing is null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task DeleteAllAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            _context.Categories.RemoveRange(categories);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
