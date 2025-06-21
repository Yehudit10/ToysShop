using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record RegisterUserDTO([Required]string Password, [Required] string UserName, string? FirstName, string? LastName);

}
