using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Reflection.Metadata.Ecma335;
namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        ToysShopContext _toysShopContext;
        public ProductRepository(ToysShopContext toysShopContext)
        {
            _toysShopContext = toysShopContext;
        }
        public async Task<IEnumerable<Product>> GetAllProducts(string? q, int? minPrice, int? maxPrice, int?[]? categories,string?order)
        {

            var products =
             _toysShopContext.Products.Where(product => (q != null ? product.Name.Contains(q) : true) &&
           (minPrice != null ? product.Price > minPrice : true) 
           && (maxPrice != null ? product.Price < maxPrice : true)
           && ((categories != null && categories.Length > 0) ? categories.Contains(product.CategoryId) : true)).OrderBy(p=>p.Id);
            if (!string.IsNullOrEmpty(order))
            {
                products = order.ToLower() switch
                {
                    "price" => products.OrderBy(p => p.Price),
                    "name" => products.OrderBy(p => p.Name),
                    _ => products
                };
            }
            return await products.ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _toysShopContext.Products.FindAsync(id);
        }
    }
}
