using LabProject.Application.Interfaces;
using LabProject.Application.Services;
using LabProject.Domain.Interfaces;
using LabProject.Infrastructure.Data;
using LabProject.Infrastructure.Data.Repositories;

namespace LabProject.WebApi.Extentions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<FileContext>();
            services.AddScoped<ICategoryRepository, FileCategoryRepository>();
            services.AddScoped<IProductRepository, FileProductRepository>();

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
