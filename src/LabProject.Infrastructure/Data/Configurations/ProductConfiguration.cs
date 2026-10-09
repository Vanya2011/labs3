using LabProject.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabProject.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Price).HasPrecision(18, 2);
            builder.Property(p => p.StockQuantity).IsRequired();

            builder.HasData(
            new Product
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Mechanical Keyboard",
                Price = 89.99m,
                StockQuantity = 25,
                CategoryId = CategoryConfiguration.ElectronicsId
            },
            new Product
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Gaming Mouse",
                Price = 49.50m,
                StockQuantity = 40,
                CategoryId = CategoryConfiguration.ElectronicsId
            },
            new Product
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "CLR via C# by Jeffrey Richter",
                Price = 54.00m,
                StockQuantity = 15,
                CategoryId = CategoryConfiguration.BooksId
            },
            new Product
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "USB-C to DisplayPort Cable",
                Price = 18.25m,
                StockQuantity = 50,
                CategoryId = CategoryConfiguration.AccessoriesId
            }
        );
        }
    }
}
