using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
namespace Repositories
{
    public interface IProductRepository
    {
         Task<IEnumerable<Product>> GetAllProducts(string? q, int? minPrice, int? maxPrice, int?[]? categories,string? order);
         Task<Product> GetProductById(int id);
    }
}
