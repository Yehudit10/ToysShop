using AutoMapper;
using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToysShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
      
        public OrdersController(IOrderService orderService)
        {
            _orderService=orderService;
           
        }
     

    

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<OrderReturnDTO> Post([FromBody] OrderDTO order)
        {
            return await _orderService.CreateOrder(order);
        }

        
    
    }
}
