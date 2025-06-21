using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public int? ErrorStatus { get; set; }
        public T? Data { get; set; }
        public static ServiceResult<T> Ok(T data) => new ServiceResult<T> { IsSuccess = true, Data = data };
        public static ServiceResult<T> Error(int status,string? errorMsg) => new ServiceResult<T> { IsSuccess = false, ErrorStatus= status,ErrorMessage=errorMsg };
    }
}
