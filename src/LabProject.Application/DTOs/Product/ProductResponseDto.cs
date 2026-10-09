using LabProject.Application.DTOs.Category;

namespace LabProject.Application.DTOs.Product
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
