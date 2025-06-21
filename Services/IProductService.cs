using DTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IProductService
    {
         Task<IEnumerable<ProductDTO>> GetAllProducts(string? q, int? minPrice, int? maxPrice, int?[]? categories,string? order);
         Task<ProductDTO> GetProductById(int id);
    }
}
