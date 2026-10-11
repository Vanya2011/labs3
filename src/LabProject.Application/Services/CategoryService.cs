using AutoMapper;
using FluentValidation;
using LabProject.Application.Common;
using LabProject.Application.DTOs.Category;
using LabProject.Application.DTOs.Product;
using LabProject.Application.Interfaces;
using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LabProject.Application.Services;

public class CategoryService : ICategoryService
{
    private const string AllCategoriesCacheKey = "categories:all";
    private static string GetCategoryCacheKey(Guid id) => $"categories:{id}";

    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCategoryDto> _createValidator;
    private readonly IValidator<UpdateCategoryDto> _updateValidator;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IValidator<CreateCategoryDto> createValidator,
        IValidator<UpdateCategoryDto> updateValidator,
        IDistributedCache cache,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var cached = await _cache.GetRecordAsync<IEnumerable<CategoryResponseDto>>(AllCategoriesCacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Cache HIT for key: {CacheKey}", AllCategoriesCacheKey);
            return cached;
        }

        _logger.LogInformation("Cache MISS for key: {CacheKey}. Fetching from database.", AllCategoriesCacheKey);
        var categories = await _categoryRepository.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories).ToList();

        await _cache.SetRecordAsync(AllCategoriesCacheKey, dtos, TimeSpan.FromMinutes(5));
        return dtos;
    }

    public async Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id)
    {
        var cacheKey = GetCategoryCacheKey(id);

        var cached = await _cache.GetRecordAsync<CategoryResponseDto>(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Cache HIT for key: {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogInformation("Cache MISS for key: {CacheKey}. Fetching from database.", cacheKey);
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return null;
        }

        var dto = _mapper.Map<CategoryResponseDto>(category);
        await _cache.SetRecordAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
        return dto;
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var category = _mapper.Map<Category>(dto);
        category.Id = Guid.NewGuid();

        var created = await _categoryRepository.AddAsync(category);

        await _cache.RemoveAsync(AllCategoriesCacheKey);
        _logger.LogInformation("Invalidated cache key: {CacheKey}", AllCategoriesCacheKey);

        return _mapper.Map<CategoryResponseDto>(created);
    }

    public async Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }

        _mapper.Map(dto, existing);
        var success = await _categoryRepository.UpdateAsync(existing);

        if (success)
        {
            await _cache.RemoveAsync(GetCategoryCacheKey(id));
            await _cache.RemoveAsync(AllCategoriesCacheKey);
            _logger.LogInformation("Invalidated cache for category ID: {CategoryId}", id);
        }

        return success;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var success = await _categoryRepository.DeleteAsync(id);
        if (success)
        {
            await _cache.RemoveAsync(GetCategoryCacheKey(id));
            await _cache.RemoveAsync(AllCategoriesCacheKey);
            _logger.LogInformation("Invalidated cache for category ID: {CategoryId}", id);
        }

        return success;
    }

    public async Task DeleteAllCategoriesAsync()
    {
        await _categoryRepository.DeleteAllAsync();
        await _cache.RemoveAsync(AllCategoriesCacheKey);
        _logger.LogInformation("Invalidated all category cache keys.");
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryIdAsync(Guid categoryId)
    {
        var products = await _categoryRepository.GetProductsByCategoryIdAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }
}