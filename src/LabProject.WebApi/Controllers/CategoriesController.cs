using LabProject.Application.Interfaces;
using LabProject.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace LabProject.WebApi.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Category>> GetById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category is null)
            {
                return NotFound($"Category with id '{id}' was not found.");
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create([FromBody] Category category)
        {
            var created = await _categoryService.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Category category)
        {
            category.Id = id;
            var updated = await _categoryService.UpdateCategoryAsync(category);
            if (!updated)
            {
                return NotFound($"Category with id '{id}' was not found.");
            }

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _categoryService.DeleteAllCategoriesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted)
            {
                return NotFound($"Category with id '{id}' was not found.");
            }

            return NoContent();
        }

        [HttpGet("{id:guid}/products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category is null)
            {
                return NotFound($"Category with id '{id}' was not found.");
            }

            var products = await _categoryService.GetProductsByCategoryIdAsync(id);
            return Ok(products);
        }
    }
}
