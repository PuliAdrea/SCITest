using Microsoft.AspNetCore.Mvc;
using SCIProducts.Application.Services.Interfaces;
using SCIProducts.Domain.Entities;

namespace SCIProductsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Obtiene todos los productos.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productService.GetAll();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving products.", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un producto por su Id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productService.GetById(id);
                if (product == null)
                    return NotFound(new { message = $"Product with id {id} not found." });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving product.", error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdProduct = await _productService.Add(product);

                if (createdProduct == null)
                    return BadRequest(new { message = "Failed to create product. Please verify the data." });

                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, new
                {
                    message = "Product created successfully.",
                    product = createdProduct
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating product.", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedProduct = await _productService.Update(id, product);

                if (updatedProduct == null)
                    return NotFound(new { message = $"Product with id {id} not found or could not be updated." });

                return Ok(new
                {
                    message = "Product updated successfully.",
                    product = updatedProduct
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating product.", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un producto por Id.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productService.Delete(id);

                if (!result)
                    return NotFound(new { message = $"Product with id {id} not found or could not be deleted." });

                return Ok(new { message = "Product deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting product.", error = ex.Message });
            }
        }
    }
}