using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LabProject.Infrastructure.Data.Repositories;


public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ApplicationDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all products from the database.");
        return await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching product with ID: {ProductId}", id);
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _logger.LogInformation("Adding product '{ProductName}' with ID: {ProductId}", product.Name, product.Id);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", product.Id);
        var existing = await _context.Products.FindAsync(product.Id);
        if (existing is null)
        {
            _logger.LogWarning("Product with ID: {ProductId} not found for update.", product.Id);
            return false;
        }

        _context.Entry(existing).CurrentValues.SetValues(product);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);
        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            _logger.LogWarning("Product with ID: {ProductId} not found for deletion.", id);
            return false;
        }

        _context.Products.Remove(product);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task DeleteAllAsync()
    {
        _logger.LogWarning("Deleting all products from the database.");
        var products = await _context.Products.ToListAsync();
        _context.Products.RemoveRange(products);
        await _context.SaveChangesAsync();
    }
}
