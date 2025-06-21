using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
namespace Tests
{
    public class CategoryRepositoryIntegrationTests: IClassFixture<DatabaseFixture>
    {
        private readonly ToysShopContext _toysShopContext;
        private readonly CategoryRepository _categoryRepository;
        public CategoryRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _toysShopContext = databaseFixture.Context;
            _categoryRepository = new CategoryRepository(_toysShopContext);
        }
        [Fact]
        public async Task GetAllCategories_ShouldReturnAllCategories()
        {
            var categories = new List<Category>
                {
                    new Category { Name = "Vehicles" },
                    new Category { Name = "Dolls" },
                    new Category { Name = "Puzzles" }
                };

            await _toysShopContext.Categories.AddRangeAsync(categories);
            await _toysShopContext.SaveChangesAsync();

            var result = await _categoryRepository.GetAllCatogeries();
            Assert.Equal(result.Count(), 3);
            Assert.Contains(result,c=>c.Name== "Vehicles");
            Assert.Contains(result,c=>c.Name== "Dolls");
            Assert.Contains(result,c=>c.Name== "Puzzles");
        }

        [Fact]
        public async Task GetAllCategories_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            _toysShopContext.Categories.RemoveRange(_toysShopContext.Categories);
            await _toysShopContext.SaveChangesAsync();

            var categories = await _categoryRepository.GetAllCatogeries();

            Assert.Empty(categories);
        }
    }
}
