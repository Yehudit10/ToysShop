﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Repositories;

namespace Services
{
    public class OrderService:IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository=orderRepository;
        }
        public async Task<IEnumerable<Order>> GetOrders(int id)
        {
            return await _orderRepository.GetOrders(id);
        }
        public async Task<Order> CreateOrder(Order order)
        {
           return await  _orderRepository.CreateOrder(order);
        }
    }
}
