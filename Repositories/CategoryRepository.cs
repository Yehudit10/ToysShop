using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ToysShopContext _toysShopContext;
        public CategoryRepository(ToysShopContext toysShopContext)
        {
            _toysShopContext = toysShopContext;
        }
        public async Task<IEnumerable<Category>> GetAllCatogeries()
        {
            return await _toysShopContext.Categories.ToListAsync();
        }
    }
}
