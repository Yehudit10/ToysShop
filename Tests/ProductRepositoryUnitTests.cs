using Entities;
using Moq;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.EntityFrameworkCore;
namespace Tests
{
    public class ProductRepositoryUnitTests
    {
        private List<Product> GetMockProducts()
        => new List<Product>
            {
                new Product { Id = 1, Name = "Lego Set", Price = 100, CategoryId = 1 },
                new Product { Id = 2, Name = "Action Figure", Price = 50, CategoryId = 2 },
                new Product { Id = 3, Name = "Puzzle", Price = 75, CategoryId = 1 },
                new Product { Id = 4, Name = "Board Game", Price = 150, CategoryId = 3 }
            };
        [Fact]
        public async Task GetAllProducts_ReturnsAll_WhenNoFilters()
        {
            // Arrange
            var mockProducts = GetMockProducts();
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Products).ReturnsDbSet(mockProducts);

            var repository = new ProductRepository(mockContext.Object);

            // Act
            var result = await repository.GetAllProducts(null, null, null, null,null);

            // Assert
            Assert.Equal(mockProducts, result);
        }

        [Fact]
        public async Task GetAllProducts_FiltersByQuery()
        {
            var mockProducts = GetMockProducts();
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Products).ReturnsDbSet(mockProducts);

            var repository = new ProductRepository(mockContext.Object);

            var result = await repository.GetAllProducts("Leg", null, null, null,null);

            Assert.Single(result);
            Assert.Equal(mockProducts[0], result.First());
        }

        [Fact]
        public async Task GetAllProducts_FiltersByMinPrice()
        {
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Products).ReturnsDbSet(GetMockProducts());

            var repository = new ProductRepository(mockContext.Object);

            var result = await repository.GetAllProducts(null, 80, null, null,null);

            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.True(p.Price > 80));
        }

        [Fact]
        public async Task GetAllProducts_FiltersByMaxPrice()
        {
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Products).ReturnsDbSet(GetMockProducts());

            var repository = new ProductRepository(mockContext.Object);

            var result = await repository.GetAllProducts(null, null, 80, null,null);

            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.True(p.Price < 80));
        }

        [Fact]
        public async Task GetAllProducts_FiltersByCategories()
        {
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Products).ReturnsDbSet(GetMockProducts());

            var repository = new ProductRepository(mockContext.Object);

            var result = await repository.GetAllProducts(null, null, null, new int?[] { 1 },null);

            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(1, p.CategoryId));
        }

        [Fact]
        public async Task GetAllProducts_CombinesAllFilters()
        {
            var mockProducts = GetMockProducts();
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Products).ReturnsDbSet(GetMockProducts());

            var repository = new ProductRepository(mockContext.Object);

            var result = await repository.GetAllProducts("Puzzle", 50, 100, new int?[] { 1 },"price");

            Assert.Single(result);
            Assert.Equal(mockProducts[2].Name, result.First().Name);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsEmpty_WhenNoMatches()
        {
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Products).ReturnsDbSet(GetMockProducts());

            var repository = new ProductRepository(mockContext.Object);

            var result = await repository.GetAllProducts("Nonexistent", null, null, null,null);

            Assert.Empty(result);
        }





    }
}
