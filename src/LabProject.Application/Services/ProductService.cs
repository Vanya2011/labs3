using AutoMapper;
using FluentValidation;
using LabProject.Application.Common;
using LabProject.Application.Configuration;
using LabProject.Application.DTOs.Product;
using LabProject.Application.Interfaces;
using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LabProject.Application.Services;

public class ProductService : IProductService
{
    private const string AllProductsCacheKey = "products:all";
    private static string GetProductCacheKey(Guid id) => $"products:{id}";

    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ProductService> _logger;
    private readonly IOptionsMonitor<PricingOptions> _pricingOptionsMonitor;
    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator,
        IDistributedCache cache,
        ILogger<ProductService> logger,
        IOptionsMonitor<PricingOptions> pricingOptionsMonitor)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _cache = cache;
        _logger = logger;
        _pricingOptionsMonitor = pricingOptionsMonitor;





        _pricingOptionsMonitor.OnChange(async (newOptions) =>
        {
            _logger.LogInformation("Pricing configuration changed dynamically. Evicting Redis cache.");
            await _cache.RemoveAsync(AllProductsCacheKey);
        });
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        var cached = await _cache.GetRecordAsync<IEnumerable<ProductResponseDto>>(AllProductsCacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Cache HIT for key: {CacheKey}", AllProductsCacheKey);
            return cached;
        }

        _logger.LogInformation("Cache MISS for key: {CacheKey}. Fetching from database.", AllProductsCacheKey);
        var products = await _productRepository.GetAllAsync();
        var dtos = products.Select(ApplyPricingRules).ToList();

        await _cache.SetRecordAsync(AllProductsCacheKey, dtos, TimeSpan.FromMinutes(5));
        return dtos;
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(Guid id)
    {
        var cacheKey = GetProductCacheKey(id);

        var cached = await _cache.GetRecordAsync<ProductResponseDto>(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Cache HIT for key: {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogInformation("Cache MISS for key: {CacheKey}. Fetching from database.", cacheKey);
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return null;
        }
        var dto = ApplyPricingRules(product);
        await _cache.SetRecordAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
        return dto;
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

        await _cache.RemoveAsync(AllProductsCacheKey);
        _logger.LogInformation("Invalidated cache key: {CacheKey}", AllProductsCacheKey);

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
        var success = await _productRepository.UpdateAsync(existing);

        if (success)
        {
            await _cache.RemoveAsync(GetProductCacheKey(id));
            await _cache.RemoveAsync(AllProductsCacheKey);
            _logger.LogInformation("Invalidated cache for product ID: {ProductId}", id);
        }

        return success;
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var success = await _productRepository.DeleteAsync(id);
        if (success)
        {
            await _cache.RemoveAsync(GetProductCacheKey(id));
            await _cache.RemoveAsync(AllProductsCacheKey);
            _logger.LogInformation("Invalidated cache for product ID: {ProductId}", id);
        }

        return success;
    }

    public async Task DeleteAllProductsAsync()
    {
        await _productRepository.DeleteAllAsync();
        await _cache.RemoveAsync(AllProductsCacheKey);
        _logger.LogInformation("Invalidated all product cache keys.");
    }
    private ProductResponseDto ApplyPricingRules(Product product)
    {
        var options = _pricingOptionsMonitor.CurrentValue;
        var dto = _mapper.Map<ProductResponseDto>(product);
        dto.OriginalPrice = product.Price;
        dto.FinalPrice = product.Price;
        dto.AppliedDiscountPercentage = 0;

        if (options.EnableGlobalDiscount && product.Price >= options.MinimumPriceForDiscount)
        {
            var discountMultiplier = options.GlobalDiscountPercentage / 100m;
            dto.FinalPrice = Math.Round(product.Price * (1.0m - discountMultiplier), 2);
            dto.AppliedDiscountPercentage = options.GlobalDiscountPercentage;
        }

        return dto;
    }
}