using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Services;
using DTOs;
//delete unused code

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToysShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: api/<CategoriesController>
        [HttpGet]
        public async Task<IEnumerable<CategoryDTO>> Get()
        {
            return await _categoryService.GetAllCategories();
        }

    }
}
