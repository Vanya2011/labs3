using AutoMapper;
using LabProject.Application.DTOs.Category;
using LabProject.Application.DTOs.Product;
using LabProject.Domain.Models;

namespace LabProject.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            CreateMap<Product, ProductResponseDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}
