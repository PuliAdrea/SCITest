using Microsoft.AspNetCore.Mvc;
using SCIProductsAPI.Models;
using SCIProductsAPI.Repositories;

namespace SCIProductsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _productRepository.CreateAsync(product);
            if (created == null)
                return StatusCode(500, new { message = "Error creating product." });

            return CreatedAtAction(nameof(GetProductById), new { id = created.Id }, created);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            var updated = await _productRepository.UpdateAsync(id, product);
            if (updated == null)
                return StatusCode(500, new { message = "Error updating product." });

            return Ok(updated);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            var deleted = await _productRepository.DeleteAsync(id);
            if (!deleted)
                return StatusCode(500, new { message = "Error deleting product." });

            return Ok(new { message = $"Product with ID {id} was deleted successfully." });
        }
    }
}
