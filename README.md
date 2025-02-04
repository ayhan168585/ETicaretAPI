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
Öncelikle Application katmanından başlayacağız çünkü önce arayüzler oluşturulur(interface) ve daha sonra concrete oluşturulur. Arayüzler Application katmanında oluşturulacağından dolayı Application katmanından başlıyoruz. Hem Application katmanında hemde persistence katmanında Repositories adında klasör oluşturuluyor. Application katmanındaki Repositories klasörü içine IRepository interface'i oluşturuluyor. Yalnız repository tasarımımızda şöyle bir değişiklik yapacağız veritabanındaki tablolara okuma yapan repositoryi ayrı yazım işlemi yapacağımız reposioryi ayrı oluşturacağız. Şimdi elimizde 3 tane IRepository var IRepository veritabanı tablolarıyla genel işlemleri, IReadRepository veritabanı tablolarındaki okuma işlemlerini, IWriteRepository veritabanı tablolarına yazma işlemlerini yapacak. Ama bazı fonksiyonlar asenkron fonksiyonları kullanacak o yüzden biz baştan asenkron tasarlayacağız.
-----------------------------
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IRepository<T> where T : class
    {
        DbSet<T> Table {  get; }
    }
}
------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IReadRepository<T>:IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetWhere(Expression<Func<T,bool>>method);
        Task<T> GetSingleAsync(Expression<Func<T,bool>>method);
        Task<T> GetByIdAsync(string Id);
    }
}
----------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IWriteRepository<T>:IRepository<T> where T : class
    {
        Task<bool> AddAsync(T model);
        Task<bool> AddAsync(List<T> model);
        Task<bool> Remove(T model);
        Task<bool> Remove(string Id);
        Task<bool> UpdateAsync(T model);


    }
}
------------------------------
şimdi de bunları implemente edecek somut nesneleri oluşturuyoruz. Ama önce şunu söylemek gerekiyor şimdiki tasarımımızda her türlü class gelebilmekte oysa bizim GetById gibi sorgularımızda bize Id gerekiyor ama bütün klaslarda Id olmak zorunda değil bu sebeple <T>'nin bir entity olduğunu belirtmemiz gerekiyor dolayısıyla ya IEntity gibi bir interface yada bizim burada yaptığımız gibi BaseEntity gibi bir class'ı marker(işaretleyici) olarak kullanabiliriz.(ikisinden birini kullanmalıyız) Bunu IRepository,IReadRepository ve IriteRepository için yapıyoruz.
Tabi bunu yapınca concrete nesnelerde buna uygun düzenlenecek
------------------------------
using ETicaretAPI.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        DbSet<T> Table {  get; }
    }
}
-----------------------------
using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IReadRepository<T>:IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetWhere(Expression<Func<T,bool>>method);
        Task<T> GetSingleAsync(Expression<Func<T,bool>>method);
        Task<T> GetByIdAsync(string Id);
    }
}
---------------------------
using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IWriteRepository<T>:IRepository<T> where T : BaseEntity
    {
       using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IWriteRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<bool> AddAsync(T model);
        Task<bool> AddRangeAsync(List<T> datas);
        bool Remove(T model);
        bool RemoveRange(List<T> datas);
        Task<bool> RemoveAsync(string Id);
        bool Update(T model);
        Task<int> SaveAsync();
    }
}
    }
}
--------------------------

Şimdi ReadRepository ve WriteRepository de şu şekilde oluşturuyoruz.
----------------------------
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
    {
        private readonly ETicaretAPIDbContext _context;
        public ReadRepository(ETicaretAPIDbContext context)
        {
            _context = context;
        }
        public DbSet<T> Table => _context.Set<T>();
        public IQueryable<T> GetAll()
            => Table;
        public IQueryable<T> GetWhere(Expression<Func<T, bool>> method)
            =>Table.Where(method);      
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> method)
            =>await Table.FirstOrDefaultAsync(method);
        public async Task<T> GetByIdAsync(string id)
            => await Table.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id));
    }
}
----------------------------
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
    {
        private readonly ETicaretAPIDbContext _context;
        public WriteRepository(ETicaretAPIDbContext context)
        {
            _context = context;
        }
        public DbSet<T> Table => _context.Set<T>();
        public async Task<bool> AddAsync(T model)
        {
         EntityEntry<T> entityEntry=  await Table.AddAsync(model);
            return entityEntry.State == EntityState.Added;            
        }
        public async Task<bool> AddRangeAsync(List<T> datas)
        {
            await Table.AddRangeAsync(datas);
            return true;
        }
        public bool Remove(T model)
        {
            EntityEntry<T> entityEntry= Table.Remove(model);
            return entityEntry.State == EntityState.Deleted;
        }
        public bool RemoveRange(List<T> datas)
        {
            Table.RemoveRange(datas);
            return true;
        }
        public async Task<bool> RemoveAsync(string id)
        {
           T model= await Table.FirstOrDefaultAsync(data=>data.Id==Guid.Parse(id));
            return Remove(model);
        }
        public bool Update(T model)
        {
            EntityEntry<T> entityEntry=Table.Update(model);
            return entityEntry?.State == EntityState.Modified;
        }
        public async Task<int> SaveAsync()
            =>await _context.SaveChangesAsync();
    }
}
---------------------------
Şimdi yapılacak şey entity lere uygun repository arayüzlerini oluşturmak ve bunların somut nesnelerini oluşturmak Arayüzler için Application katmanında Repository klasörünün içine her bir entity için bir klasör açıyoruz ama açarken entity ismi veriyorsak entity ismi ile aynı olmamsı için çoğul klasör ismi kullanıyoruz aksi taktirde entitylerle namespace isimleri aynı olduğunda entitylere erişemeyiz aynı mantıkla persistence katmanı içinde repositories klasöründe entity isimlerinin çoğul halleriyle klasör açıyoruz.

her bir entity için arayüz isimleri ICustomerReadRepository ve ICustomerWriteRepository(Customer için) diğer entityler içinde aynı mantıkla 2 interface oluşturuyoruz. 
persistence katmanındaki isimleri ise CustomerReadRepository ve CustomerWriteRepository(customer için) diğer entityler içinde aynı mantıkla 2 class oluşturuyoruz.

Sadece customer için yapacağım diğerleri de aynı mantık
----------------------------
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories.Customers
{
    public interface ICustomerReadRepository:IReadRepository<Customer>
    {
    }
}
-------------------------
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories.Customers
{
    public interface ICustomerWriteRepository:IWriteRepository<Customer>
    {
    }
}
--------------------------
Somut nesneler
----------------------------
using ETicaretAPI.Application.Repositories.Customers;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories.Customers
{
    public class CustomerReadRepository : ReadRepository<Customer>, ICustomerReadRepository
    {
        public CustomerReadRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
------------------------
using ETicaretAPI.Application.Repositories.Customers;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories.Customers
{
    public class CustomerWriteRepository : WriteRepository<Customer>, ICustomerWriteRepository
    {
        public CustomerWriteRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
---------------------------
şimdi yapılması gereken IoC conteynıra burada yaptığımız yapılanmayı eklemek
-------------------------
AddSingleton yada scope olarak ekleyebiliriz biz scope olarak ekleyeceğiz
------------------------
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
          services.AddDbContext<ETicaretAPIDbContext>(options=>options.UseNpgsql(Configuration.ConnectionString),ServiceLifetime.Singleton);
            services.AddSingleton<ICustomerReadRepository, CustomerReadRepository>();
            services.AddSingleton<ICustomerWriteRepository, CustomerWriteRepository>();
            services.AddSingleton<IOrderWriteRepository, OrderWriteRepository>();
            services.AddSingleton<IOrderReadRepository, OrderReadRepository>();
            services.AddSingleton<IProductReadRepository, ProductReadRepository>();
            services.AddSingleton<IProductWriteRepository, ProductWriteRepository>();
        }
    }
}

--------------------------
ve en son ProductControlleri şu şekilde düzenliyoruz bakalım ürünler geliyormu ama AddScoped ürün oluşturma sırasında hata verdiğinden yukarıdaki şekilde düzeltildi.
------------------------
using ETicaretAPI.Application.Repositories.Products;
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
        public IActionResult GetProducts() 
        {
        var result=_productReadRepository.GetAll();
            return Ok(result);        
        }
    }
}
----------------------------
Bunu yapınca ürünler listesi boş şekilde geliyor çünkü veritabanında ürün yok.
----------------------------
productsController ürün oluşturma sırasında DateTime.Now hata verdiğinden DateTime.UtcNow şekline dönüştürüldü.
---------------------------
using ETicaretAPI.Application.Repositories.Products;
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
        public async void Get()
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
    }
}
---------------------------
Bu düzeltmelerden sonra (ServiceRegistration ve UtcNow) veritabanına 4 adet ürün eklendi.

DÜZELTMELER
-------------------------
Şimdi daha önce hata verdiği ServiceRegistration ilk durumuyla ilgili AddScoped iken hatanın çözümü 
öncelikle AddScoped iken Dispose hatası alıyorduk. Bunun nedeni Scoped'de request edilen her nesne işi bittikten sonra dispose edilir. ProductsController de örnek product oluşturmak için kullanılan Get fonksiyonu asenkron tanımlanmadığı için ürün oluşturma sırasında _productWriteResponse dispose edildiğinden hata veriyor. Bunu Addscoped kullandığımızda productsController deki Get fonksiyonunu Task ile kullanmaktır. Serviceregistration'u ilk haline çeviriyor ve controlleri task ile düzeltiyoruz.
---------------------------
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

-------------------------
using ETicaretAPI.Application.Repositories.Products;
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
    }
}
---------------------------
peki singleton da neden hata vermedi çünkü scoped da oluşturulan nesne işi bittikten sonra hemen dispose olduğundan (asenkronda ise verinin gelmesini bekliyor) void kullanılırken veri gelmeden evvel dispose olduğundan hata verdi singletonda ise işlem bitse bile dispose olmadığından hata vermedi peki hangisini kullanmalıyız Addscoped olarak kullanılması daha sağlılı bu sebeple problemi yukarıdaki şekilde çözüyoruz. Bir sonraki çözümümüz GetById de marker kullanarak sorunu çözmüştük şimdi bir başka çözüm yöntemini görelim. FindAsync kullanarak çözöyoruz
---------------------------
 public async Task<T> GetByIdAsync(string id)
     //=> await Table.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id));
     => await Table.FindAsync(Guid.Parse(id));
------------------------------
Productscontroller de yeni bir HttpGet ile bunu bir kontrol edelim.
----------------------------
  [HttpGet("{id}")]

  public async Task<IActionResult> Get(string id)
  {
      Product product=await _productReadRepository.GetByIdAsync(id);
      return Ok(product);
  }
  ----------------------------
  Şimdi EntityFrameork vasıtasıyla yapılan sorgularla çekilen verilerin takip edilmesini sağlayan tracking sistemi üzerine bir çalışma yapacağız

  ENTITY FRAMEORK CORE TRACKING SİSTEMİ
  ---------------------------------------
  DbContext vasıtasıyla veri tabanında yapılan işlemler üzerinde tracking mekanizması otomatik olarak çalışır ve her işlemi izler.Veriler üzerinde herhangi bir manipülasyon(update,delete vb) yapmayacaksak tracking sisteminin devre dışı bırakılması daha performanslı çalışmaya yarayacaktır diyelimki 1000 tane ürünümüz var ve bunun listelenmesi komutunu veriyorum eğer bu 1000 tane ürün üzerinde herhangi bir manipülasyon işlemi yoksa tracking sisteminin devre dışı bırakılması daha hızlı çalışmasına sebep olur.
  --------------------------
  using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IReadRepository<T>:IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll(bool tracking=true);
        IQueryable<T> GetWhere(Expression<Func<T,bool>>method, bool tracking = true);
        Task<T> GetSingleAsync(Expression<Func<T,bool>>method, bool tracking = true);
        Task<T> GetByIdAsync(string Id, bool tracking = true);
    }
}
-------------------------
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
    {
        private readonly ETicaretAPIDbContext _context;
        public ReadRepository(ETicaretAPIDbContext context)
        {
            _context = context;
        }
        public DbSet<T> Table => _context.Set<T>();
        public IQueryable<T> GetAll(bool tracking = true)
        {
            var query=Table.AsQueryable();
            if(!tracking)
                query=query.AsNoTracking();
            return query;
        }
        public IQueryable<T> GetWhere(Expression<Func<T, bool>> method,bool tracking=true)
           {
            var query=Table.Where(method);
            if(!tracking)
                query=query.AsNoTracking();
            return query;
        }      
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = Table.AsNoTracking();
             return await query.FirstOrDefaultAsync(method);
        }
        public async Task<T> GetByIdAsync(string id, bool tracking = true)
        //=> await Table.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id));
        //=> await Table.FindAsync(Guid.Parse(id));
        {
            var query=Table.AsQueryable();
            if(!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(data=>data.Id==Guid.Parse(id));
        }
    }
}

----------------------------
Şimdi yine bir optimizasyon gerçekleştireceğiz bu optimizasyon veri ekleme işlemi ile olacak oda veri tabanına veri ekleme sırasında entitylerin ortak olan alanlarının merkezi bir yerden eklenmesi sağlanacak. Bu bir interceptor yoluyla olacak bu şu şekilde olacak diyelimki bir product eklemek istiyoruz. eklenme esnasında interceptor araya girecek ve diyecek ki (benzetme sanatıyla anlatıyorum.) bende ürüne ait şu verilerde var al bu verileride senin verilerine ekle ve öyle veritabanına gönder. BaseEntity'e bir property daha ekliyoruz. UpdatedDate adında bir property. Şimdi entityler ile ilgili herhangi bir değişiklik yapınca bunun sunucuya bildirilmesi gerekiyor. add-migration mig_2 komutunu veriyoruz çünkü mig_1 i daha önce yapmıştık update-database ile de değişikliğimizi veritabanına yansıtıyoruz. Şimdi yapacağımız iş eğer bir veri ilkkez ekleniyorsa createdDate'in eklenmesi eğer bir veri güncelleniyorsa updatedDate'in eklenmesini sağlıyoruz peki bunu nerede yapıyoruz Context nesnesi üzerinde savechangeAsync sırasında yapılan işlemi anlayarak ona uygun alanın veri tabanına eklenmesini sağlıyoruz bunu yapmak için tracker özelliğinden yararlanıyoruz.
---------------------------
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Common;
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
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var datas = ChangeTracker.Entries<BaseEntity>();
            foreach (var data in datas)
            {
                _ = data.State switch (Burada boş eşleme yapıyoruz.)
                {
                    EntityState.Added => data.Entity.CreatedDate=DateTime.UtcNow,
                    EntityState.Modified=>data.Entity.UpdatedDate=DateTime.UtcNow,
                };
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
--------------------------
Bundan sonra hem client hemde backend üzerinde işlem yapacağız. ilgili klasörde ki bizim için ProjelerinAngularHali adlı klasör  ng new ETicaretClient --no-standalone ile EticaretClient adında bir proje oluşturuyoruz. ve visualcode ile açıyoruz. (Bunun için oluşan ETicaretClient klasörüne gidip cmd yi açmak ve code . komutunu vermek) Angularda oluşturduğumuz yapı hem Admin hemde kullancı kısmının bulunduğu multi-layout bir yapı oluşturacağız. Admin kısmında Material UI kısmında bootstrap kullanacağız. Admin kısmında Product,Order,Customer,Dashboard sayfaları olacak UI kısmında Product,Home,Basket sayfaları olacak. Şimdi bizim anabir modülümüz var AppModule bu modülde angular içinde kullanılan tüm modül ve componentlerin declare ve importları mevcut olmalıdır. Biz burada modüler bir yapı kullanarak oluşum yapacağız. Bu sebeple öncelikle 2 modül oluşturacağız Admin modülü ve UI modülü. Angularda bir modüle yada component oluşturmak istediğimizde öncelikle klasörünü oluşturur ve içine oluşturmak istediğimiz module yada componenti oluşturur. Şimdi addmin modülünü oluşturalım komut satırımız şu şekilde olacak ng generate module admin yada bunun kısaltmalarıyla ng g m admin şeklinde oluşturulur. bunu yapınca önce admin klasörü oluşturulur ve içine admin modülü oluşturulur. Aynı şekilde ng g m ui ile UI modülünü oluşturuyoruz.
  







