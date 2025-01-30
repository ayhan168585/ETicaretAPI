using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{
    public static class Configuration
    {
        public static string ConnectionString {


            get
            {
                using IHost host = Host.CreateApplicationBuilder().Build();

                IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

                string connectionString = config.GetValue<string>("ConnectionStrings:PostgreSQL");

                return connectionString;
            }
                
                
                }
    }
}
