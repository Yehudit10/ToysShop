using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DTOs
{
    public record UserDTO([Required] int Id, [Required] string UserName,string FirstName,string LastName);
    
}
