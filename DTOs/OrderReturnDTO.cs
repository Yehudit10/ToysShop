using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record OrderReturnDTO(int Id, [Required] int UserId, DateTime? Date, double? OrderSum, ICollection<OrderItemDTO> OrderItems);

}
