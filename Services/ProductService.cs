
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Entities;
using Microsoft.EntityFrameworkCore.Metadata;
using AutoMapper;
using DTOs;
namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository,IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductDTO>> GetAllProducts(string? q, int? minPrice, int? maxPrice, int?[]? categories, string? order)
        {
            IEnumerable<Product> products = await _productRepository.GetAllProducts(q, minPrice, maxPrice, categories,order);
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductById(int id)
        {
            Product product= await _productRepository.GetProductById(id);
            return _mapper.Map<ProductDTO>(product);
        }
    }
}
