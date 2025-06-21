using Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
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
    public class OrderRepositoryUnitTests
    {


        [Fact]
        public async Task CreateOrder_AddsOrderAndSaves()
        {
            var newOrder = new Order { Id = 4, UserId = 103, OrderSum = 150 };

            var mockContext = new Mock<ToysShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockContext.Setup(c => c.Orders).ReturnsDbSet(new List<Order>());
            mockContext.Setup(c => c.Orders.AddAsync(It.IsAny<Order>(), default))
    .Returns(new ValueTask<EntityEntry<Order>>((EntityEntry<Order>?)null));
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.CreateOrder(newOrder);

            Assert.Equal(newOrder, result);
            mockContext.Verify(c => c.Orders.AddAsync(newOrder, default), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_ThrowsException_WhenContextFails()
        {
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Orders.AddAsync(It.IsAny<Order>(), default)).ThrowsAsync(new Exception("DB insert error"));

            var repository = new OrderRepository(mockContext.Object);

            await Assert.ThrowsAsync<Exception>(() => repository.CreateOrder(new Order { Id = 5 }));
        }

        [Fact]
        public async Task CreateOrder_ThrowsException_WhenOrderIsNull()
        {
            var mockContext = new Mock<ToysShopContext>();
            var repository = new OrderRepository(mockContext.Object);

            await Assert.ThrowsAsync<NullReferenceException>(() => repository.CreateOrder(null!));
        }
    }
}
