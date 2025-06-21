using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DTOs;
using Entities;
using Repositories;

namespace Services
{
    public class OrderService:IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public OrderService(IOrderRepository orderRepository,IMapper mapper)
        {
            _orderRepository=orderRepository;
            _mapper = mapper;
        }
      
        public async Task<OrderReturnDTO> CreateOrder(OrderDTO orderToCreate)
        {
            Order order = _mapper.Map<Order>(orderToCreate);
            Order createdOrder= await _orderRepository.CreateOrder(order);
            OrderReturnDTO returnOrder = _mapper.Map<OrderReturnDTO>(createdOrder);
            return returnOrder;
        }
    }
}
