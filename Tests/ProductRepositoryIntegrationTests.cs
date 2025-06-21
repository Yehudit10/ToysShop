using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class ProductRepositoryIntegrationTests: IClassFixture<DatabaseFixture>,IAsyncLifetime
    {
        private readonly ToysShopContext _toysShopContext;
        private readonly ProductRepository _productRepository;

      
        public ProductRepositoryIntegrationTests(DatabaseFixture fixture)
        {
            _toysShopContext = fixture.Context;
            _productRepository = new ProductRepository(_toysShopContext);
            
        }

        private async Task SeedTestData()
        {
            if (!_toysShopContext.Products.Any())
            {


                // Add test categories first
                var categories = new List<Category>
        {
            new Category { Name = "Building Blocks" },
            new Category { Name = "Dolls" },
            new Category { Name = "Vehicles" },
            new Category { Name = "Action Figures" },
            new Category { Name = "Games & Puzzles" }
        };

                await _toysShopContext.Categories.AddRangeAsync(categories);
                await _toysShopContext.SaveChangesAsync();

                // Get the actual category IDs after insertion
                var categoryIds = await _toysShopContext.Categories
                    .OrderBy(c => c.Name)
                    .Select(c => c.Id)
                    .ToListAsync();

                // Add test products (don't set Id - let it auto-generate)
                var products = new List<Product>
        {
            new Product { Name = "Lego Castle", Price = 50, CategoryId = categoryIds[1] }, // Building Blocks
            new Product { Name = "Barbie Doll", Price = 25, CategoryId = categoryIds[2] }, // Dolls
            new Product { Name = "Hot Wheels Car", Price = 5, CategoryId = categoryIds[4] }, // Vehicles
            new Product { Name = "Lego Spaceship", Price = 75, CategoryId = categoryIds[1] }, // Building Blocks
            new Product { Name = "Action Figure", Price = 15, CategoryId = categoryIds[0] }, // Action Figures
            new Product { Name = "Puzzle 1000 pieces", Price = 20, CategoryId = categoryIds[3] }, // Games & Puzzles
            new Product { Name = "Remote Control Car", Price = 100, CategoryId = categoryIds[4] }, // Vehicles
            new Product { Name = "Teddy Bear", Price = 30, CategoryId = categoryIds[2] }, // Dolls
            new Product { Name = "Board Game", Price = 40, CategoryId = categoryIds[3] }, // Games & Puzzles
            new Product { Name = "Toy Robot", Price = 35, CategoryId = categoryIds[0] } // Action Figures
        };

                await _toysShopContext.Products.AddRangeAsync(products);
                await _toysShopContext.SaveChangesAsync();
            }
        }
        public async Task InitializeAsync()
        {
            await SeedTestData();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
        #region GetAllProducts Tests

        [Fact]
        public async Task GetAllProducts_WithNoFilters_ReturnsAllProducts()
        {
            // Act
          
            var result = await _productRepository.GetAllProducts(null, null, null, null, null);

            // Assert
            Assert.Equal(10, result.Count());
            // Products should be ordered by Id by default (first inserted product should be first)
            Assert.Equal("Lego Castle", result.First().Name);
        }

        [Fact]
        public async Task GetAllProducts_WithSearchQuery_ReturnsMatchingProducts()
        {
           

            // Act
            var result = await _productRepository.GetAllProducts("Lego", null, null, null, null);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Contains("Lego", p.Name));
        }

        [Fact]
        public async Task GetAllProducts_WithSearchQueryCaseInsensitive_ReturnsMatchingProducts()
        {
         
            // Act
            var result = await _productRepository.GetAllProducts("lego", null, null, null, null);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Contains("Lego", p.Name));
        }

        [Fact]
        public async Task GetAllProducts_WithSearchQueryNoMatches_ReturnsEmptyList()
        {
            // Act
            var result = await _productRepository.GetAllProducts("NonExistentProduct", null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllProducts_WithMinPrice_ReturnsProductsAboveMinPrice()
        {
            
            // Act
            var result = await _productRepository.GetAllProducts(null, 30, null, null, null);

            // Assert
            Assert.Equal(5, result.Count());
            Assert.All(result, p => Assert.True(p.Price > 30));
        }

        [Fact]
        public async Task GetAllProducts_WithMaxPrice_ReturnsProductsBelowMaxPrice()

        {
            // Act
           
            var result = await _productRepository.GetAllProducts(null, null, 30, null, null);

            // Assert
            Assert.Equal(4, result.Count());
            Assert.All(result, p => Assert.True(p.Price < 30));
        }

        [Fact]
        public async Task GetAllProducts_WithMinAndMaxPrice_ReturnsProductsInRange()
        {
        
            // Act

            var result = await _productRepository.GetAllProducts(null, 20, 50, null, null);

            // Assert
            Assert.Equal(4, result.Count());
            Assert.All(result, p => Assert.True(p.Price > 20 && p.Price < 50));
        }

        [Fact]
        public async Task GetAllProducts_WithInvalidPriceRange_ReturnsEmptyList()
        {
         
            // Act (minPrice > maxPrice)
            var result = await _productRepository.GetAllProducts(null, 50, 30, null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllProducts_WithSingleCategory_ReturnsProductsInCategory()
        {
          
            // Arrange - Get the Building Blocks category ID
            var buildingBlocksCategory = await _toysShopContext.Categories
                .FirstAsync(c => c.Name == "Building Blocks");

            // Act
            var result = await _productRepository.GetAllProducts(null, null, null, new int?[] { buildingBlocksCategory.Id }, null);

            // Assert
            Assert.Equal(2, result.Count()); // Lego Castle and Lego Spaceship
            Assert.All(result, p => Assert.Equal(buildingBlocksCategory.Id, p.CategoryId));
            Assert.Contains(result, p => p.Name == "Lego Castle");
            Assert.Contains(result, p => p.Name == "Lego Spaceship");
        }

        [Fact]
        public async Task GetAllProducts_WithMultipleCategories_ReturnsProductsInAnyCategory()
        {
         
            // Arrange - Get Building Blocks and Dolls category IDs
            var categories = await _toysShopContext.Categories
                .Where(c => c.Name == "Building Blocks" || c.Name == "Dolls")
                .Select(c => c.Id)
                .ToListAsync();
            int?[] categoriesArray = categories.Select(i => (int?)i).ToArray();

            // Act
            var result = await _productRepository.GetAllProducts(null, null, null, categoriesArray, null);

            // Assert
            Assert.Equal(4, result.Count()); // 2 Lego products + 2 Doll products
            Assert.All(result, p => Assert.Contains(p.CategoryId, categoriesArray));
        }

        [Fact]
        public async Task GetAllProducts_WithNonExistentCategory_ReturnsEmptyList()
        {
         
            // Act
            var result = await _productRepository.GetAllProducts(null, null, null, new int?[] { 999 }, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllProducts_WithEmptyCategoryArray_ReturnsAllProducts()
        {
        
            // Act
            var result = await _productRepository.GetAllProducts(null, null, null, new int?[] { }, null);

            // Assert
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public async Task GetAllProducts_OrderByPrice_ReturnsSortedByPrice()
        {
      
            // Act
            var result = await _productRepository.GetAllProducts(null, null, null, null, "price");

            // Assert
            var resultList = result.ToList();
            Assert.Equal(10, resultList.Count);
            Assert.Equal(5, resultList.First().Price); // Hot Wheels Car
            Assert.Equal(100, resultList.Last().Price); // Remote Control Car

            // Verify ascending order
            for (int i = 1; i < resultList.Count; i++)
            {
                Assert.True(resultList[i].Price >= resultList[i - 1].Price);
            }
        }

        [Fact]
        public async Task GetAllProducts_OrderByName_ReturnsSortedByName()
        {
         
            // Act
            var result = await _productRepository.GetAllProducts(null, null, null, null, "name");

            // Assert
            var resultList = result.ToList();
            Assert.Equal(10, resultList.Count);
            Assert.Equal("Action Figure", resultList.First().Name);
            Assert.Equal("Toy Robot", resultList.Last().Name);

            // Verify alphabetical order
            for (int i = 1; i < resultList.Count; i++)
            {
                Assert.True(string.Compare(resultList[i].Name, resultList[i - 1].Name, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        [Fact]
        public async Task GetAllProducts_OrderByCaseInsensitive_ReturnsSortedByPrice()
        {
  
            // Act
            var result = await _productRepository.GetAllProducts(null, null, null, null, "PRICE");

            // Assert
            var resultList = result.ToList();
            Assert.Equal(5, resultList.First().Price);
            Assert.Equal(100, resultList.Last().Price);
        }

        [Fact]
        public async Task GetAllProducts_OrderByInvalidValue_ReturnsDefaultOrder()
        {
  
            // Act
            var result = await _productRepository.GetAllProducts(null, null, null, null, "invalid");

            // Assert
            var resultList = result.ToList();
            Assert.Equal(10, resultList.Count);
            Assert.Equal(1, resultList.First().Id); // Should maintain default order by Id
        }

        [Fact]
        public async Task GetAllProducts_OrderByEmptyString_ReturnsDefaultOrder()
        {
        
            // Act
            var result = await _productRepository.GetAllProducts(null, null, null, null, "");

            // Assert
            var resultList = result.ToList();
            Assert.Equal(10, resultList.Count);
            Assert.Equal(1, resultList.First().Id); // Should maintain default order by Id
        }

        [Fact]
        public async Task GetAllProducts_CombinedFilters_ReturnsCorrectResults()
        {
      
            // Act - Search for "Car" with price between 0-50 in category 3
            var result = await _productRepository.GetAllProducts("Car", 0, 50, new int?[] { 3 }, "price");

            // Assert
            Assert.Single(result);
            Assert.Equal("Hot Wheels Car", result.First().Name);
            Assert.Equal(5, result.First().Price);
            Assert.Equal(3, result.First().CategoryId);
        }

        [Fact]
        public async Task GetAllProducts_AllFiltersNoMatch_ReturnsEmptyList()
        {
      
            // Act - Impossible combination
            var result = await _productRepository.GetAllProducts("Lego", 200, 300, new int?[] { 999 }, null);

            // Assert
            Assert.Empty(result);
        }

        

        #endregion

        #region GetProductById Tests

        [Fact]
        public async Task GetProductById_ExistingId_ReturnsProduct()
        {
  
            // Act
            var result = await _productRepository.GetProductById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Lego Castle", result.Name);
            Assert.Equal(50, result.Price);
            Assert.Equal(1, result.CategoryId);
        }

        [Fact]
        public async Task GetProductById_NonExistingId_ReturnsNull()
        {
   
            // Act
            var result = await _productRepository.GetProductById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductById_NegativeId_ReturnsNull()
        {
   
            // Act
            var result = await _productRepository.GetProductById(-1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductById_ZeroId_ReturnsNull()
        {
       
            // Act
            var result = await _productRepository.GetProductById(0);

            // Assert
            Assert.Null(result);
        }

     

        #endregion

        #region Edge Cases and Performance Tests

        [Fact]
        public async Task GetAllProducts_VeryLongSearchString_HandledGracefully()
        {
          
            // Act
            var longString = new string('a', 1000);
            var result = await _productRepository.GetAllProducts(longString, null, null, null, null);

            // Assert
            Assert.Empty(result);
        }

      

       

        [Fact]
        public async Task GetAllProducts_MaxIntPrices_HandledCorrectly()
        {
            

            // Act
            var result = await _productRepository.GetAllProducts(null, int.MaxValue, int.MaxValue, null, null);

            // Assert
            Assert.Empty(result); // No products should have price > int.MaxValue
        }

        [Fact]
        public async Task GetProductById_MaxIntId_ReturnsNull()
        {
          

            // Act
            var result = await _productRepository.GetProductById(int.MaxValue);

            // Assert
            Assert.Null(result);
        }

        #endregion


    }
}
