using ETicaretAPI.Application.Repositories.Customers;
using ETicaretAPI.Application.Repositories.Orders;
using ETicaretAPI.Application.Repositories.Products;
using ETicaretAPI.Persistence.Contexts;
using ETicaretAPI.Persistence.Repositories.Customers;
using ETicaretAPI.Persistence.Repositories.Orders;
using ETicaretAPI.Persistence.Repositories.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace ETicaretAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
          services.AddDbContext<ETicaretAPIDbContext>(options=>options.UseNpgsql(Configuration.ConnectionString));
              
            services.AddScoped<ICustomerReadRepository,CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository,CustomerWriteRepository>();
            services.AddScoped<IOrderWriteRepository,OrderWriteRepository>();
            services.AddScoped<IOrderReadRepository,OrderReadRepository>();
            services.AddScoped<IProductReadRepository,ProductReadRepository>();
            services.AddScoped<IProductWriteRepository,ProductWriteRepository>();
        }
    }
}
