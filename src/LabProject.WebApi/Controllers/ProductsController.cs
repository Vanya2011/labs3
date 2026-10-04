using LabProject.Application.Interfaces;
using LabProject.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace LabProject.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Product>> GetById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound($"Product with id '{id}' was not found.");
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            var category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);
            if (category is null)
            {
                return BadRequest($"Category with id '{product.CategoryId}' does not exist.");
            }

            var created = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Product product)
        {
            product.Id = id;

            var category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);
            if (category is null)
            {
                return BadRequest($"Category with id '{product.CategoryId}' does not exist.");
            }

            var updated = await _productService.UpdateProductAsync(product);
            if (!updated)
            {
                return NotFound($"Product with id '{id}' was not found.");
            }

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _productService.DeleteAllProductsAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
            {
                return NotFound($"Product with id '{id}' was not found.");
            }

            return NoContent();
        }
    }
}
