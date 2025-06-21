using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using DTOs;
namespace Services
{
    public interface IOrderService
    {
      
        Task<OrderReturnDTO> CreateOrder(OrderDTO order);
    }
}
