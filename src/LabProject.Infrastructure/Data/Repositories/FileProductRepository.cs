using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;

namespace LabProject.Infrastructure.Data.Repositories
{
    public class FileProductRepository : IProductRepository
    {
        private readonly FileContext _context;
        private readonly string _productsFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "products.json");

        public FileProductRepository(FileContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            var items = _context.Read<Product>(_productsFilePath);
            return Task.FromResult<IEnumerable<Product>>(items);
        }

        public Task<Product?> GetByIdAsync(Guid id)
        {
            var items = _context.Read<Product>(_productsFilePath);
            var product = items.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<Product> AddAsync(Product product)
        {
            var items = _context.Read<Product>(_productsFilePath);
            if (product.Id == Guid.Empty)
            {
                product.Id = Guid.NewGuid();
            }

            items.Add(product);
            _context.Write(_productsFilePath, items);
            return Task.FromResult(product);
        }

        public Task<bool> UpdateAsync(Product product)
        {
            var items = _context.Read<Product>(_productsFilePath);
            var index = items.FindIndex(p => p.Id == product.Id);
            if (index == -1)
            {
                return Task.FromResult(false);
            }

            items[index].Name = product.Name;
            items[index].Price = product.Price;
            items[index].StockQuantity = product.StockQuantity;
            items[index].CategoryId = product.CategoryId;

            _context.Write(_productsFilePath, items);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var items = _context.Read<Product>(_productsFilePath);
            var product = items.FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return Task.FromResult(false);
            }

            items.Remove(product);
            _context.Write(_productsFilePath, items);
            return Task.FromResult(true);
        }

        public Task DeleteAllAsync()
        {
            _context.Write(_productsFilePath, new List<Product>());
            return Task.CompletedTask;
        }
    }
}
