using ETicaretAPI.Application.Repositories.Products;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductWriteRepository _productWriteRepository;


        public ProductsController(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }



        [HttpGet]
        public async Task Get()
        {
            await _productWriteRepository.AddRangeAsync(new()
            {
                new(){Id=Guid.NewGuid(),Name="Product 1",Stock=10,Price=100,CreatedDate=DateTime.UtcNow},
                new(){Id=Guid.NewGuid(),Name="Product 2",Stock=10,Price=200,CreatedDate=DateTime.UtcNow},
                new(){Id=Guid.NewGuid(),Name="Product 3",Stock=10,Price=300,CreatedDate=DateTime.UtcNow},
                new(){Id=Guid.NewGuid(),Name="Product 4",Stock=10,Price=400,CreatedDate=DateTime.UtcNow}



            });
            await _productWriteRepository.SaveAsync();

        }

        [HttpGet("{id}")]

        public async Task<IActionResult> Get(string id)
        {
            Product product=await _productReadRepository.GetByIdAsync(id);
            return Ok(product);
        }

    }
}
