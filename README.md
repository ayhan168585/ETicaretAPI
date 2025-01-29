ONİON ARCHİTECTURE İLE ETİCARET UYGULAMASI (GENÇAY YILDIZ TARAFINDAN YAPILAN)

ETicaretAPI Projesi
--------------------------
Öncelikle ETiceretAPI adında blank solution oluşturuyoruz.Bunun içine Application ve Domain katmanını içinde barındıracak olan Core klasörü oluşturulur. İçine add komutu ve new project ile Domain adında bir class library projesi oluşturuyoruz ama bunu yaparken genellikle önce solüsyonun adı nokta ve projenin adı şeklinde oluşturulur. Bu sebeple katmanın adı ETicaretAPI.Domain adını veriyoruz. Location kısmında core klasörünün ismi görülmüyor bu sebeple core klasörünü biz ekliyoruz ve projeyi(katmanı) oluşturuyoruz. Diğer katmanımız ETicaretAPI.Application böylece Core klasöründeki katmanlarımızı tamamlamış olduk. Şimdi solüsyon üzerinde Infrastructure adında bir klasör oluşturuyoruz ve içine class library ile ETiceretAPI.Infrastructure ve ETicaretAPI.Persistence katmanları oluşturulur. Böylece Infrastructure klasörü ve içinde katmanları oluşturuldu geriye presentation katmanı kaldı. Bunun için önce solüsyon üzerinde Presentation adında bir klasör ve ETicaretAPI.API adında ASP.NET Core WebAPI katmanı oluşturuyoruz. Böylece tüm katmanlarımız oluşturuldu. Onion architecture için bu katman mimarisinin olması gerekli ancak yeterli değildir. Katmanlar arasındaki ilişkilerin düzenlenmesi ve yapıların (Entities,concrete,interface vb.) doğru katmanlarda oluşturulması gerekli
--------------------------------------
Application katmanı Domain katmanını referans edecek
Infrastructure katmanı Application katmanını referans edecek
Persistence katmanı Application katmanını referans edecek
Presentation katmanı Application katmanını referans edecek
Presentation katmanı Infrastructure katmanını referans edecek
Presentation katmanı Persistence katmanını referans edecek
----------------------------------------
Domain katmanında Entities adında bir klasör oluşturuyoruz burada veritabanı nesnelerimizi tutacağız ancak burada tüm entitylerin ortak alanlarını örneğin id,createdDate vb. property lerin tutulacağı bir genel baseentity clsssının tutulması için Entities Klasörünün içine Common adında bir klasör daha oluşturuyoruz. Şimdi Common klasöründe BaseEntity adında bir class oluşturuyoruz.
------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities.Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
--------------------------------
Entities klasöründe Product adında bir class oluşturuyoruz.
--------------------------------
using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public long Price { get; set; }

    }
}
-----------------------------------
Şimti bu entity'i diğer katmanlara açacak bir interface'e ihtiyacımız olacak.Mesela tüm productları getiren fonksiyonun soyut yapısını Application katmanında oluşturacağız bu sebeple Application katmanında Abstractions adında bir klasör oluşturuyoruz. ileride bu yapıları daha profesyonel olarak yapacağız şimdilik yapımızın çalışıp çalşımadığını kontrol edecek dummy datalar ile çalışacağımızdan yapıyı şimdilik kuruyoruz.
------------------------------
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions
{
    public interface IProductService
    {
        List<Product> GetProducts();
    }
}
-----------------------------
Şimdi bu soyut yapılanmanın concrete nesnesini oluşturacağız. Bunu ya persistence yada infrastructure da oluşturacağız eğer kullanacağımız yapı veritabanını ilgilendiriyorsa Persistence katmanında veritabanını ilgilendirmiyorsa infrastructure da oluşturacağız. Şimdi her ne kadar hazır veri ile çalışsakta product bir veri tabanı nesnesidir bu sebeple oluşturacağımız concrete nesnesi Persistence katmanında olacak Persistence katmanı içine concretes adında bir klasör oluşturuyoruz. ve içine ProductService adında bir class oluşturuyoruz ve IProductService'den implemens ediyoruz. Ancak hazır veriyle çalışcağımızdan oluşturuken target type özelliğiyle oluşturacağız.
------------------------------
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
---------------------------
dummy datalarımızı oluşturduk. Ayrıca Application katmanımızda interface'imizi ve persistence katmanında concrete nesnesini oluşturduk. şimdi yapacağımız presentation katmanının controller klasöründe products adında bir controller oluşturuyoruz. Ve bu productscontroller deki metodu tetikleyecek bir yapı oluşturalım. API katmanımızda bir tane IoC Conteinerimiz var işte bu IoC containere diyeceğizki eğer senden bir IProductService nesnesi istenirse bize ProductService nesnesi ver peki bunu nasıl yapacağız Persistence katmanına yada infrastructure katmanına IoC Container'a tanımlama yapmamızı sağlayan registration fonksiyonları tanımlıyoruz. Bu fonksiyonlar extension fonksiyonlar olacak persistence katmanının içine ServiceRegistration class'ı oluşturuyoruz. Bu sınıf hem public hemde static olacak çünkü biz bunun içinde extension fonksiyon tanımlayacağız. ve bunu program.cs dosyasında services.AddControllers() vb diğer servicelerdeki gibi IServiceCollection'a bağlamamız gerekiyor. IServiceCollection'u kullanabilmek için Microsoft.Extensions.DependencyInjection.Abstraction paketinin yüklenmesi gerekiyor. Bunu visualstudionun yeni versiyonlarında açılan kutucuktan install diyerek yükleyebiliriz. 
---------------------------
using ETicaretAPI.Application.Abstractions;
using ETicaretAPI.Persistence.Concretes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddSingleton<IProductService, ProductService>();
        }
    }
}
-----------------------------
Ama oluşturduğumuz bu serviceRegistration classı içindeki AddPersistenceServices extension fonksiyonunu program.cs dosyası içinde tanımlamamız gerekir.
-----------------------------
builder.Services.AddPersistenceServices();
----------------------------
eklemesini yapıyoruz ve şimdi productsControllerimizi de şu şekilde yazınca
---------------------------
using ETicaretAPI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _productService.GetProducts();
            return Ok(products);
        }
    }
}
-----------------------------
Böylece dummy data ile çalışsakta onion architecture sistemi doğru kurulduğundan dolayı ürünler getirilebildi. Şimdi veritabanı alt yapılarını hazırlayalım. Ama önce dummy data ile ilgili tüm yapıları siliyoruz. Öncelikle 3 tablo üzerinden gideceğiz product,order ve customer tabloları
----------------------------
using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public long Price { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
------------------------------
using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class Order:BaseEntity
    {
        public string Description { get; set; }
        public string Adress { get; set; }
        public int CustomerId { get; set; }
        public ICollection<Product> Productions { get; set; }
        public Customer Customer { get; set; }
    }
}
------------------------------
şimdilik entitylerimiz bu kadar şimdi de context nesnesi oluşturalım.Persistence katmanında Contexts adında bir klasör oluşturuyoruz. ve içine ETicaretAPIDbContext adında bir context nesnesi oluşturuyoruz bu sınıfı oluşturuyoruz ama bunun bir context olabilmesi için DbContext ten kalıtım alması gerekiyor ama bunu yapabilmemiz için Microsft.EntityFrameworkCore paketinin yüklenmesi gerekir.
----------------------------
using ETicaretAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Contexts
{
    public class ETicaretAPIDbContext : DbContext
    {
        public ETicaretAPIDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
--------------------------------
Tabi bunun IoC Conteynıra bildirilmesi gerekiyor. Bunun için ServiceRegistration sınıfını kullanacağız. Bu servis registration sınıfıyla program.cs dosyasında kullanacağımız IoC Conteynıra context'i bildiriyoruz daha önce dummy data ile çalışırkende productService ve IProductService bildirmiştik. Burada postgreSql i çağıracağız ama bunun için bir paket yüklememiz gerekiyor. Manage nuget package ile Npgsql.EntityFrameworkCore.PostgreSQL paketini yüklüyoruz.
-------------------------------
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<ETicaretAPIDbContext>(options => options.UseNpgsql("User ID=postgres;Password=12345;Host=localhost;Port=5432;Database=ETicaretAPIDb"));
        }
    }
}
-----------------------------
şimdi sırada migration işlemi var. Öncelikle manage nuget package den persistence üzerinde Microsoft.EntityFrameworkCore.Tools ve Microsoft.EntityFrameworkCore.Design paketlerini yüklüyoruz.Bu paketleri hem persistence katmanına hemde presentation katmanına yüklüyoruz.

MIGRATION KOMUTLARI
-----------------------
enable-migration--------------->Migrationu aktif hale getirir.
add-migration mig_1------------>mig_1 adında migration oluşturulur.
update-database---------------->Database oluşur.
--------------------------

Biz kodlarımızın hiç bir yerinde bilgilerimizi girmememiz gerekiyor dolayısıyla connection stringi bu şekilde girmeyeceğiz burada bir düzenleme yapacağız. Ama önce eğer biz bu migration komutlarını cmd yada powershell üzerinden yapacaksak kodumuzu şu şekilde düzenliyoruz. öncelikle Persistence katmanında DesignTimeDbContextFactory adında bir class oluşturuyoruz.
-----------------------------
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ETicaretAPIDbContext>
    {
        public ETicaretAPIDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ETicaretAPIDbContext> dbContextOptionBuilder = new();
            dbContextOptionBuilder.UseNpgsql("User ID=postgres;Password=12345;Host=localhost;Port=5432;Database=ETicaretAPIDb");
            return new (dbContextOptionBuilder.Options);
        }
    }
}
----------------------------
powershell de komut 
dotnet ef migrations add mig_1-----------------> Ama bu kod bende çalışmadı ef yi tanımadı
dotnet ef database update
-------------------------------
Biz migration komutlarını visual studio içindeki package manager console 'dan yapıyoruz. Komutlarımızdan sonra veritabanımız oluşuyor. Şimdi connection stringin açık şekilde yazılmasını düzeltelim. Bu tip düzeltmeler json dosyaları üzerinden yapılır. Bizim hali hazırda bir json dosyamız var. appsettings.json dosyası bunun üzerinde düzeltilecek ve ilgili yerde buradan çağırılacak. Ancak burada Gençay hocanın kullanmış olduğu versiyonda kullandığı ConfigurationManager sınıfı Benim kullandığım NET:8'de çalışmadı oyüzden şu şekilde düzenleme yaptım. Yine şu 2 paket yüklenecek Microsoft.Extensions.Configuration ve Microsoft.Extensions.Configuration.Json ama bunun yanında benim yaptığım düzenleme için ek olarak Microsoft.Extensions.Hosting, Microsoft.Extensions.Hosting.Abstractions paketleri de yüklenecek bundan sonra öncelikle appsettings.json dosyasında şu düzenlemeyi yapıyoruz ardından persistence katmanında Configuration adlı static bir sınıf oluşturuluyor.
------------------------------
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
    "ConnectionStrings" : {
    "PostgreSQL": "User ID=postgres;Password=12345;Host=localhost;Port=5432;Database=ETicaretAPIDb;"
  }
}

------------------------------
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
        public static string ConnectionString 
        {
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
--------------------------
Bunu nerede kullanmak istersek buradan çekip alıyoruz. ServiceRegistration sınıfına şu şekilde alıyoruz.
-------------------------
using ETicaretAPI.Application.Abstractions;
using ETicaretAPI.Persistence.Concretes;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {            
            services.AddSingleton<IProductService,ProductService>();
           services.AddDbContext<ETicaretAPIDbContext>(options=>options.UseNpgsql(Configuration.ConnectionString));
        }
    }
}
----------------------------
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ETicaretAPIDbContext>
    {
        public ETicaretAPIDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ETicaretAPIDbContext> dbContextOptionBuilder = new();
            dbContextOptionBuilder.UseNpgsql(Configuration.ConnectionString);
            return new (dbContextOptionBuilder.Options);
        }
    }
}
-----------------------------
Migration ve veritabanını siliyoruz ve tekrar migration yapıyoruz böylece connection stringe ulaşabildiğimizi ve veritabanının oluştuğunu görüyoruz. Şimdi bir sonraki işlemimiz veri erişim modelini tasarlamak 

GENERİC REPOSITORY DESİGN PATTERN 
----------------------------------
Öncelikle Application katmanından başlayacağız çünkü önce arayüzler oluşturulur(interface) ve daha sonra concrete oluşturulur. Arayüzler Application katmanında oluşturulacağından dolayı Application katmanından başlıyoruz. Hem Application katmanında hemde persistence katmanında Repositories adında klasör oluşturuluyor. Application katmanındaki Repositories klasörü içine IRepository interface'i oluşturuluyor. Yalnız repository tasarımımızda şöyle bir değişiklik yapacağız veritabanındaki tablolara okuma yapan repositoryi ayrı yazım işlemi yapacağımız reposioryi ayrı oluşturacağız.











