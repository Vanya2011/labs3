using AutoMapper;
using FluentValidation;
using LabProject.Application.DTOs.Category;
using LabProject.Application.DTOs.Product;
using LabProject.Application.Interfaces;
using LabProject.Domain.Interfaces;
using LabProject.Domain.Models;

namespace LabProject.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCategoryDto> _createValidator;
    private readonly IValidator<UpdateCategoryDto> _updateValidator;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IValidator<CreateCategoryDto> createValidator,
        IValidator<UpdateCategoryDto> updateValidator)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
    }

    public async Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category is null ? null : _mapper.Map<CategoryResponseDto>(category);
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
        return await _categoryRepository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        return await _categoryRepository.DeleteAsync(id);
    }

    public async Task DeleteAllCategoriesAsync()
    {
        await _categoryRepository.DeleteAllAsync();
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryIdAsync(Guid categoryId)
    {
        var products = await _categoryRepository.GetProductsByCategoryIdAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }
}
