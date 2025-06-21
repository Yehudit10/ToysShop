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
    public class OrderRepositoryIntegrationTests: IClassFixture<DatabaseFixture>
    {

        private readonly ToysShopContext _toysShopContext;
        private readonly OrderRepository _orderRepository;
        public OrderRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _toysShopContext = databaseFixture.Context;
            _orderRepository = new OrderRepository(_toysShopContext);
        }
      
        
        [Fact]
        public async Task CreateOrder_ShouldAddOrderToDatabase()
        {
            await _toysShopContext.Users.AddAsync(new User { UserName = "aaa", Password = "92u34bv" });
            await _toysShopContext.SaveChangesAsync();
           var user = _toysShopContext.Users.First();

            var newOrder = new Order
            {
                UserId = user.Id,
                Date = DateTime.UtcNow,
                OrderSum = 100
            };

            var result = await _orderRepository.CreateOrder(newOrder);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);

            var dbOrder = await _toysShopContext.Orders.FindAsync(result.Id);
            Assert.NotNull(dbOrder);
            Assert.Equal(user.Id, dbOrder.UserId);
            Assert.Equal(newOrder.OrderSum, dbOrder.OrderSum);
            Assert.Equal(newOrder.Date, dbOrder.Date);
        }
    }
}
