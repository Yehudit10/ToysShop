using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Services;
using System.Threading.Tasks;
//delete unused code

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToysShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<IEnumerable<ProductDTO>> Get(string? q,int? minPrice,int? maxPrice, [FromQuery] int?[]? categories, string? order)
        {
            
            return await _productService.GetAllProducts(q,minPrice,maxPrice,categories,order);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<ProductDTO> Get(int id)
        {
            return await _productService.GetProductById(id);
        }   
    }
}
