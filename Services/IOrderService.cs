using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
namespace Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrders(int id);
        Task<Order> CreateOrder(Order order);
    }
}
