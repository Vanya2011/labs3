using Microsoft.EntityFrameworkCore;
using LabProject.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LabProject.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public static readonly Guid ElectronicsId = Guid.Parse("a3d2e5b1-285e-4c8d-9b5f-8c38a2e7c001");
        public static readonly Guid BooksId = Guid.Parse("b4e3f6c2-396f-4d9e-0c6a-9d49b3f8d002");
        public static readonly Guid AccessoriesId = Guid.Parse("c5f4a7d3-407a-4eaf-1d7b-0e5ac4a9e003");
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(500);

            builder.HasMany(c => c.Products)
                  .WithOne(p => p.Category)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
            builder.HasData(
            new Category
            {
                Id = ElectronicsId,
                Name = "Electronics",
                Description = "Electronic devices and digital equipment"
            },
            new Category
            {
                Id = BooksId,
                Name = "Books",
                Description = "Technical literature and textbooks"
            },
            new Category
            {
                Id = AccessoriesId,
                Name = "Accessories",
                Description = "Peripherals, cables, and hardware components"
            }
        );
        }
    }
}
