using Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.EntityFrameworkCore;
using Repositories;
namespace Tests
{
    public class CategoryRepositoryUnitTests
    {

        [Fact]
        public async Task GetCategories_returnsCategories()
        {
            // Arrange
            var mockCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Action Figures" },
                new Category { Id = 2, Name = "Board Games" }
            };

            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(x => x.Categories).ReturnsDbSet(mockCategories);

            var repository = new CategoryRepository(mockContext.Object);

            // Act
            var result = await repository.GetAllCatogeries();

            // Assert
         
            Assert.Equal(mockCategories, result);

        }
        [Fact]
        public async Task GetAllCatogeries_ReturnsEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Categories).ReturnsDbSet(new List<Category>());

            var repository = new CategoryRepository(mockContext.Object);

            // Act
            var result = await repository.GetAllCatogeries();

            // Assert
            Assert.NotNull(result); 
            Assert.Empty(result);
        }



    }
}
