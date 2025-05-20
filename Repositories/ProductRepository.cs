using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        ToysShopContext _toysShopContext;
        public ProductRepository(ToysShopContext toysShopContext)
        {
            _toysShopContext = toysShopContext;
        }
        public async Task<IEnumerable<Product>> GetAllProducts()//get vs update db
        {
           return await _toysShopContext.Products.ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _toysShopContext.Products.FindAsync(id);
        }
    }
}
