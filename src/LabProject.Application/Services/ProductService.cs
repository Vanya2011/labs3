using AutoMapper;
using FluentValidation;
using LabProject.Application.DTOs.Product;
using LabProject.Application.Interfaces;
using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;

namespace LabProject.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product is null ? null : _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category is null)
        {
            throw new KeyNotFoundException($"Category with ID '{dto.CategoryId}' does not exist.");
        }

        var product = _mapper.Map<Product>(dto);
        product.Id = Guid.NewGuid();

        var created = await _productRepository.AddAsync(product);
        return _mapper.Map<ProductResponseDto>(created);
    }

    public async Task<bool> UpdateProductAsync(Guid id, UpdateProductDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existing = await _productRepository.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category is null)
        {
            throw new KeyNotFoundException($"Category with ID '{dto.CategoryId}' does not exist.");
        }

        _mapper.Map(dto, existing);
        return await _productRepository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        return await _productRepository.DeleteAsync(id);
    }

    public async Task DeleteAllProductsAsync()
    {
        await _productRepository.DeleteAllAsync();
    }
}
