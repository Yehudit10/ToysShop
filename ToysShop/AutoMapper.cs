using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entities;
namespace DTOs
{
    public class AutoMapper:Profile
    {
        public AutoMapper()
        {
           
            
            CreateMap<LoginUserDTO, User>();
            CreateMap<OrderItemDTO, OrderItem>().ReverseMap();
            CreateMap<User, UserDTO>();
            CreateMap<RegisterUserDTO, User>();
            CreateMap<Category, CategoryDTO>();
            CreateMap< OrderDTO,Order>(); 
            CreateMap<Order, OrderReturnDTO>();
            CreateMap<Product, ProductDTO>();




        }
    }
}
