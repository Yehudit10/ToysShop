﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IProductService
    {
         Task<IEnumerable<Product>> GetAllProducts();
         Task<Product> GetProductById(int id);
    }
}
