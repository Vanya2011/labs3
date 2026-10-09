using AutoMapper;
using LabProject.Application.DTOs.Category;
using LabProject.Application.Interfaces;
using LabProject.Application.Services;
using LabProject.Domain.Interfaces;
using LabProject.Infrastructure.Data;
using LabProject.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace LabProject.WebApi.Extentions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<FileContext>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UpdateCategoryDto).Assembly));
            return services;
        }
    }
}
