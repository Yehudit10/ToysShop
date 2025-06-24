using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record OrderItemDTO(int ProductId,int Quantity);
    //delete unused code
    //public record OrderItemDTO([Required]int Id,string ProductName,double Price,string Description,string ImageUrl,int Quantity);
}
