using ETicaretAPI.Application.Abstractions;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Concretes
{
    public class ProductService : IProductService
    {
        public List<Product> GetProducts() 
            => new() 
        {
            new(){Id=Guid.NewGuid(),CreatedDate=DateTime.Now,Name="Product 1",Stock=5,Price=100},
            new(){Id=Guid.NewGuid(),CreatedDate=DateTime.Now,Name="Product 2",Stock=10,Price=50},
            new(){Id=Guid.NewGuid(),CreatedDate=DateTime.Now,Name="Product 3",Stock=15,Price=200},
            new(){Id=Guid.NewGuid(),CreatedDate=DateTime.Now,Name="Product 4",Stock=12,Price=170},
            new(){Id=Guid.NewGuid(),CreatedDate=DateTime.Now,Name="Product 5",Stock=20,Price=75},
        };
       
    }
}
