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
        // GET: api/<OrdersController>
        [HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public async Task<IEnumerable<Order>> Get(int id)
        {
            return await _orderService.GetOrders(id);
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<Order> Post([FromBody] Order order)
        {
            return await _orderService.CreateOrder(order);
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

    
    }
}
