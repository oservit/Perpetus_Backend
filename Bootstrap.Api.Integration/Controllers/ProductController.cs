using Core.Application.Samples.DTOs;
using Core.Application.Samples.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Central.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        [HttpGet("ListAll")]
        public async Task<IActionResult> ListAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost("InsertAndPublish")]
        public async Task<IActionResult> InsertAndPublish(
            [FromBody] CreateProductDto dto)
        {
            var id =
                await _service.InsertWithEventAsync(dto);

            return Ok(new
            {
                Id = id,
                Message = "Produto criado e evento publicado."
            });
        }
    }
}
