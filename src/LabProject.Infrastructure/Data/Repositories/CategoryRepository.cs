using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LabProject.Infrastructure.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryRepository> _logger;

    public CategoryRepository(ApplicationDbContext context, ILogger<CategoryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all categories from the database.");
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching category with ID: {CategoryId}", id);
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> AddAsync(Category category)
    {
        _logger.LogInformation("Adding category '{CategoryName}' with ID: {CategoryId}", category.Name, category.Id);
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId}", category.Id);
        var existing = await _context.Categories.FindAsync(category.Id);
        if (existing is null)
        {
            _logger.LogWarning("Category with ID: {CategoryId} not found for update.", category.Id);
            return false;
        }

        _context.Entry(existing).CurrentValues.SetValues(category);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
        var category = await _context.Categories.FindAsync(id);
        if (category is null)
        {
            _logger.LogWarning("Category with ID: {CategoryId} not found for deletion.", id);
            return false;
        }

        _context.Categories.Remove(category);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task DeleteAllAsync()
    {
        _logger.LogWarning("Deleting all categories from the database.");
        var categories = await _context.Categories.ToListAsync();
        _context.Categories.RemoveRange(categories);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
    {
        _logger.LogInformation("Fetching products for Category ID: {CategoryId}", categoryId);
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync();
    }
}