using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
namespace Repositories
{
    public class OrderRepository:IOrderRepository
    {
        private readonly ToysShopContext _toysShopContext;
        public OrderRepository(ToysShopContext toysShopContext)
        {
            _toysShopContext = toysShopContext;
        }
        public async Task<Order> CreateOrder(Order order)
        {
            await _toysShopContext.Orders.AddAsync(order);
            await _toysShopContext.SaveChangesAsync();
            return order;
        }
        public async Task<IEnumerable<Order>> GetOrders(int id)
        {
            return await _toysShopContext.Orders.Where(order => order.UserId == id).ToListAsync();
        }
    }
}
