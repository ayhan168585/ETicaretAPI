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
Bundan sonra hem client hemde backend üzerinde işlem yapacağız. ilgili klasörde ki bizim için ProjelerinAngularHali adlı klasör  ng new ETicaretClient --no-standalone ile EticaretClient adında bir proje oluşturuyoruz. ve visualcode ile açıyoruz. (Bunun için oluşan ETicaretClient klasörüne gidip cmd yi açmak ve code . komutunu vermek) Angularda oluşturduğumuz yapı hem Admin hemde kullancı kısmının bulunduğu multi-layout bir yapı oluşturacağız. Admin kısmında Material UI kısmında bootstrap kullanacağız. Admin kısmında Product,Order,Customer,Dashboard sayfaları olacak UI kısmında Product,Home,Basket sayfaları olacak. Şimdi bizim anabir modülümüz var AppModule bu modülde angular içinde kullanılan tüm modül ve componentlerin declare ve importları mevcut olmalıdır. Biz burada modüler bir yapı kullanarak oluşum yapacağız. Bu sebeple öncelikle 2 modül oluşturacağız Admin modülü ve UI modülü. Angularda bir modüle yada component oluşturmak istediğimizde öncelikle klasörünü oluşturur ve içine oluşturmak istediğimiz module yada componenti oluşturur. Şimdi addmin modülünü oluşturalım komut satırımız şu şekilde olacak ng generate module admin yada bunun kısaltmalarıyla ng g m admin şeklinde oluşturulur. bunu yapınca önce admin klasörü oluşturulur ve içine admin modülü oluşturulur. Aynı şekilde ng g m ui ile UI modülünü oluşturuyoruz. Şimdi biz sitenin adresini yazdığımızda ilk açılacak ekran UI ekranı olacak. ng g m admin/layout adında bir layout oluşturacağız. Böylece admin klasörü altında layout isimli bir modül oluşacak adminmodule içine layoutmodülün import edilmesi gerekir. Layout içinde bir layout componentimiz olması gerekiyor. Bir componentin kullanılabilmesi için uygulamanın ana modülüne(app.module) declare edilmesi gerekir. Bu sebeple öncelikle kendisine en yakın modüle (Layout component için bu layout modül) declare edilir. Layout modül admin modüle import edilecek admin modülde app modüle import edilecek ve böylece hiyerarşik olaral layout component admin modüle tanıtılmış olacak. Layout modül içinde yönetim panelinin componentleri olacak peki bunu nasıl yapıyoruz. eğer bir şeyin componentleri olacaksa önece components isimli bir modül oluşturup içine componentleri oluşturacağız ng g m admin/layout/components komutu ile components modülünü oluşturuyoruz. Şimdi bunun içine componentleri oluşturacağız. Bu compoenentlerden biri header ng g c admin/layout/components/header komutu ile header adında component oluşturuyoruz. Şimdi bu component en yakın modül olan component modüle eklendi component modülüde layout modüle ekliyoruz. şimdi bir de sidebar component oluşturuyoruz. Birde footer adında compoenent oluşturuyoruz. Bizim component modüldeki header,sidebar ve footer componentlerini layout component de selector olarak kullanılabilmesi için component modülün export edilmesi gerekir şimdi sırayla öncelikle headercomponenti componentmodüle export ediyoruz daha sonra  layout modülde layout componenti export ediyoruz. admin modülde layout modülü export ediyoruz. ve en son olarak app modülde de admin modülün import edilmesi gerekiyor.
--------------------------------
components.module
-------------------
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { FooterComponent } from './footer/footer.component';



@NgModule({
  declarations: [
    HeaderComponent,
    SidebarComponent,
    FooterComponent
  ],
  imports: [
    CommonModule
  ],
 exports:[
  HeaderComponent

 ]
})
export class ComponentsModule { }
---------------------------------

layout.Module
----------------
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './layout.component';
import { ComponentsModule } from './components/components.module';



@NgModule({
  declarations: [
    LayoutComponent
  ],
  imports: [
    CommonModule,
    ComponentsModule
  ],
  exports:[
    LayoutComponent
  ]
  })

export class LayoutModule { }
--------------------------------
admin.Module
-------------
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutModule } from './layout/layout.module';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    LayoutModule
  ],
  exports:[
     LayoutModule
   ]
})

export class AdminModule { }
-------------------------------
app.module
-------------
import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AdminModule } from './admin/admin.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    AdminModule
  ],
 
  providers: [
    provideClientHydration(withEventReplay())
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
-------------------------------
Böylece Layout Modüle app modül tarafından tanınır. Aynı uygulamayı sidebar ve footer içinde yapıyoruz. Layout componentlerinin yanında admin kısmındada product,order vb. componentlerimiz olacak ama bu adminin içinde olacak bu sebeple admin içinde components adında bir modül oluşturuyoruz. ng g m admin/components böylece admin içinde components modülünü oluşturduk şimdi bunun içinde products,orders vb. componentleri oluşturacağız ama bunuda önce components içinde products modülünü oluşturup içine products componentini oluşturacağız diğerleri içinde aynı şekilde bir uygulama yapacağız. ng g m admin/components/products komutu ile admin/components içinde products modülünü oluşturuyoruz. Bu oluşturduğumuz modülleri components modüle import ediyoruz.
-----------------------------
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductsModule } from './products/products.module';
import { OrdersModule } from './orders/orders.module';
import { CustomersModule } from './customers/customers.module';
import { DashboardModule } from './dashboard/dashboard.module';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ProductsModule,
    OrdersModule,
    CustomersModule,
    DashboardModule
  ]
})
export class ComponentsModule { }
------------------------------------
components modülde bir üst modül olan admin modüle eklenir.
---------------------------
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutModule } from './layout/layout.module';
import { ComponentsModule } from './components/components.module';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    LayoutModule,
    ComponentsModule
  ],
  exports:[
     LayoutModule
   ]
})

export class AdminModule { }
-------------------------------
Böylece hiyararşik olarak componentler modüle declare edildi modülde bir üst modüle o da bir üst modüle import edildi. Admin kısmının alt yapısı tamamlandı şimdi UI kısmını yapalım.UI kısmında layout denen altyapıyı oluşturmaya gerek yok çünkü uygulamanın default layoutunu kullanacak çünkü ilk açılacak kısım UI kısım olacak.Direkt components kısımlarını oluşturup devam edeceğiz. ng g m ui/components komutuyla UI içinde components modülü oluşturuyoruz. Bunun içine önce products modülü ve içine de products componenti oluşturuyoruz. Orderlarla ilgili kullanıcıya gösterilen sayfada bir şey göstermeye gerek yok.(Gösterilmemeli) Örneğin ne olabilir sepet olabilir ana sayfa olabilir bunlarıda oluşturuyoruz aynı mantıkla önce modül içine component. Böylece hem Admin hemde UI kısmının alt yapısını oluşturduk şimdi multiple layout altyapısını oluşturalım. Bunu şu şekilde yapacağız kaç tane layout kullanırsak kullanalım bu layoutların herbirindeki componentlere rota belirleyeceğiz.Add routing den değil oradanda yapabiliriz ama biz modüle bağlı componentler üzerinden tanımlayacağız. Örneğin admindeki Customer module şu şekilde rota veriyoruz.
-----------------------------
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomersComponent } from './customers/customers.component';
import { RouterModule } from '@angular/router';



@NgModule({
  declarations: [
    CustomersComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild([
      {path:"",component:CustomersComponent}
    ])
  ]
})
export class CustomersModule { }
---------------------------------
diğer componentler için de rotaları aynı şekilde oluşturuyoruz. UI Kısım içinde oluşturuyoruz. Her bir componente modül seviyesinde bir rota belirledik. Birinci aşama componentlerin kendi modüllerinde rotalanmasıydı bu bitti Şimdi ikinci aşamaya geçiyoruz.İkinci aşama Bu kendi modüllerinde yapılan rotalandırmanın ana modülde yapılmasıdır. Bunu şu şekilde yapıyoruz ana layout dışında kalan layoutları rotalandırıyoruz en son ana layoutu rotalandırıyoruz. app.routing de admin rotalarını şu şekilde oluşturuyoruz.
---------------------------------
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from './admin/layout/layout.component';
import { DashboardComponent } from './admin/components/dashboard/dashboard/dashboard.component';
import { HomeComponent } from './ui/components/home/home/home.component';

const routes: Routes = [
  {
    path: 'admin',
    component: LayoutComponent,
    children: [
      { path: '', component: DashboardComponent },
      {
        path: 'customers',
        loadChildren: () =>
          import('./admin/components/customers/customers.module').then(
            (module) => module.CustomersModule
          ),
      },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./admin/components/dashboard/dashboard.module').then(
            (module) => module.DashboardModule
          ),
      },
      {
        path: 'orders',
        loadChildren: () =>
          import('./admin/components/orders/orders.module').then(
            (module) => module.OrdersModule
          ),
      },
      {
        path: 'products',
        loadChildren: () =>
          import('./admin/components/products/products.module').then(
            (module) => module.ProductsModule
          ),
      },
    ],
  },
  { path: '', component: HomeComponent },
  {
    path: 'basket',
    loadChildren: () =>
      import('./ui/components/baskets/baskets.module').then(
        (module) => module.BasketsModule
      ),
  },
  {
    path: 'products',
    loadChildren: () =>
      import('./ui/components/products/products.module').then(
        (module) => module.ProductsModule
      ),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}

-----------------------------------
 {path:"",component:DashboardComponent}, bir layoutun ana sayfasında kullanılacak component direk bu şekilde tanımlanır eğer admin ana sayfası istenirse dashboard gelsin demektir bu.

Ana layout da ise obje olarak ve direkt component olarak tanımlanır. Böylece route alt yapısıda tamamlandı yeni componentler olursa onlarında eklenmesi gerekiyor. son olarak bunu app.component.html de şu şekilde kullanıyoruz.
-----------------------------------
<a routerLink="">Home</a><a routerLink="products">Products</a
><a routerLink="basket">Basket</a><a routerLink="admin">Admin</a>

<br>
<router-outlet></router-outlet>
------------------------------
buradaki <router-outlet></router-outlet> ile sayfalar gösteriliyor. Bir sayfada <router-outlet></router-outlet> in kullanılabilmesi için bağlı olduğu modülde RouterModülün import edilmesi gereklidir. Layout.component.html dosyasını şu şekilde kullanıyoruz.
-------------------------------
<app-header></app-header>
<app-sidebar></app-sidebar>
<app-footer></app-footer>

<router-outlet></router-outlet>
---------------------------------------
burada <router-outlet><router-outlet> in kullanılabilmesi için layout modüle RouterModülün eklenmesi gerekir.
----------------------------------
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './layout.component';
import { ComponentsModule } from './components/components.module';
import { RouterModule } from '@angular/router';



@NgModule({
  declarations: [
    LayoutComponent
  ],
  imports: [
    CommonModule,
    ComponentsModule,
    RouterModule
  ],
  exports:[
    LayoutComponent
  ]
  })

export class LayoutModule { }
-------------------------------
Bende bir hata vermedi ve düzgün çalıştı ama Gençay hocanın kullandığı angular versiyonunda app.module.ts dosyasına BrowserAnimadionModule import edilmediğinden sayfalar arası geçiş yapılamadı bu sebeple bizde bu modülü ekliyoruz.
-------------------------------
import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AdminModule } from './admin/admin.module';
import { UiModule } from './ui/ui.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    AdminModule,
    UiModule
  ],
 
  providers: [
    provideClientHydration(withEventReplay())
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
--------------------------------

Admin kısmının tasarımında Material UI kısmının tasarımında Bootstrap kullanacağız. Şimdi bu altyapıyı ekleyelim. Öncelikle material install edilecek ng add @angular/material
komutuyla install ediyoruz orada bize tasarım seçimiyle ilgili sorular soracak sonra daha sonra bir componenti kullanmak istersek örnek olarak layout.html de sidenav componentini kullanalim öncelikle html sayfasına 
--------------------------------
<mat-drawer-container class="example-container">
    <mat-drawer mode="side" opened>Drawer content</mat-drawer>
    <mat-drawer-content>Main content</mat-drawer-content>
  </mat-drawer-container>
  ----------------------------
  ama buradaki kütüphaneleri kullanabilmek için https://material.angular.io/components/sidenav/api sayfasındaki https://material.angular.io/components/sidenav/api kodu html sayfası hangi modüle bağlıysa bu import oraya import edilecek burada layoutModule
  -----------------------------
  import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './layout.component';
import { ComponentsModule } from './components/components.module';
import { RouterModule } from '@angular/router';
import {MatSidenavModule} from '@angular/material/sidenav';



@NgModule({
  declarations: [
    LayoutComponent
  ],
  imports: [
    CommonModule,
    ComponentsModule,
    RouterModule,
    MatSidenavModule
  ],
  exports:[
    LayoutComponent
  ]
  })

export class LayoutModule { }
---------------------------
diğer kullanacağımız componentler de buna benzer eklenecek. Ama bunu ekleyince hala eksik olan bir şey var onuda material sayfasındaki sidenav kısmında görüyoruz ve css de eklememiz gereken kodu söylüyor bunu da layout.compoennet.css dosyasına ekliyoruz.
--------------------------
.example-container {
    width: auto;
    height: 200px;
    margin: 10px;
    border: 1px solid #555;
    /* The background property is added to clearly distinguish the borders between drawer and main
       content */
    background: #eee;
  }
  --------------------------
  Bunu da ekleyince görüntü düzeliyor. Biz burada ifadeleri kendimize uygun şekilde düzeltiyoruz.
  -------------------------------
<app-header></app-header>
<mat-drawer-container class="admin-container">
    <mat-drawer mode="side" opened class="admin-left"><app-sidebar></app-sidebar></mat-drawer>
    <mat-drawer-content><router-outlet></router-outlet></mat-drawer-content>
  </mat-drawer-container>
  <app-footer></app-footer>
  ----------------------------
 .admin-container {
    width: auto;
    height: 600px;
    margin: 10px;
    border: 1px solid #555;
    /* The background property is added to clearly distinguish the borders between drawer and main
       content */
    background: #eee;
  }

  .admin-left{
    width: 250px;
  }
  ---------------------------
  Burada kullandığımız sidebar içinde sidebar componentini kullanmak istiyoruz.
  -----------------------------
  Bunu yapınca sol tarafta görünen sidebar daki ifadelere tıklayınca sağda o sayfa ile ilgili veri gelir. Admin ile ilgili material altyapısını oluşturduk. Şimdi UI kısmı için Bootstrap altyapısını düzenleyeceğiz. npm install bootstrap@5.3.3 komutuyla bootstrapin 5.3.3 versiyonunu yüklüyoruz. npm üzerinden yüklenilen paketler node_module klasörüne yüklenir. yüklendikten sonra node_module klasörü içinde bootstrap klasöründeki dist klasöründe css klasöründe css ler kullanılır ve js klasörü içindeki javascriptler kullanılır. Bunlar angular.json dosyasında styles kısmına ve scripts kısmına eklenecek.
  --------------------------------
  "styles": [
              "@angular/material/prebuilt-themes/azure-blue.css",
              "src/styles.css",
              "node_modules/bootstrap/dist/css/bootstrap.min.css"
            ],
            "scripts": [
              "node_modules/bootstrap/dist/js/bootstrap.min.js",
              "node_modules/@popperjs/core/dist/umd/popper.min.js",
               "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
            ],
-----------------------------------
Angular 19 da navbar seçilir menünün işlemesi için scripts kısmı bu şekilde düzenlenmeli. Angular üzerinde herhangi bir değişiklik yapılırsa angular ng serve ile tekrar başlatmak gerekir.
 npm i jquery ile jquery de yüklüyoruz. angular.json dosyasında script kısmına "node_modules/jquery/dist/jquery.min.js" yapıştırılıyor.Angular için alt yapımızı büyük oranda tamamladık bir kaç kütüphane daha yükleyip anguları daha güzel hale getireceğiz. Bunlardan birisi de alertify alertify ile dialog pencereleri yada notification pencereleri oluşturacağız. Ama bunu biz bu projede sadece admin kısmında ve sadece notification kısmını kullanacağız. npm install alertifyjs --save komutu ile yüklüyoruz ve node_module içinde alertify içinde build içinde 
 alertify.min.js dosyasını bularak sağ tıklıyoruz ve copy relative path ile yolunu kopyalıyoruz ve angular.json dosyasında script kısmına yapıştırıyoruz ancak yapıştırdığımızda slash lar ters olarak geliyor ve biz onları   "node_modules/alertifyjs/build/alertify.min.js" şeklinde düzelterek son haline getiriyoruz. aynı şekilde css dosyalarınıda css klasöründen alarak styles kısmına yapıştırıyoruz. Son olarak birde tema seçerek styles kısmına yapıştıralım biz burada semantic seçtik
 ---------------------------
   "styles": [
              "@angular/material/prebuilt-themes/azure-blue.css",
              "src/styles.css",
              "node_modules/bootstrap/dist/css/bootstrap.min.css",
              "node_modules/alertifyjs/build/css/alertify.min.css",
              "node_modules/alertifyjs/build/css/themes/semantic.min.css"
            ],
            "scripts": [
              "node_modules/bootstrap/dist/js/bootstrap.min.js",
              "node_modules/@popperjs/core/dist/umd/popper.min.js",
              "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js",
              "node_modules/jquery/dist/jquery.min.js",
              "node_modules/alertifyjs/build/alertify.min.js"
--------------------------------
Şimdi biz bu alertify'i kullanımımıza uygun hale getireceğiz. Bu ve bunun gibi kütüphaneleri kendimize uygun hale getirmek için bir servis oluşturuyoruz. https://alertifyjs.com/notifier.html dökümantasyon sayfasında kullanabileceğimiz metotlar yazmakta Şimdi bir servis yazacağız. şöyle bir strateji uygulayacağız uygulamamızda ana dizinde app klasöründe services adında bir klasör olacak ve altında admin ve ui adında iki klasör olacak adminde kullanacağım servisleri admine ui da kullanacağım servisleri ui kısmına yazacağız. yada her ikisinde birden kullanacağımız commen klasörü olacak. ng g s services/admin/alertify komutu ile services klasöründe bir admin klasörü oluşuyor ve içinde alertify adında bir class oluşuyor. 
-------------------------------
import { Injectable } from '@angular/core';
declare var alertify:any

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

  constructor() { }

  message(message:string,messageType:MessageType){
    alertify[messageType](message)
  }
}

export enum MessageType{
  Error="error",
  Message="message",
  Notify="notify",
  Success="success",
  Warning="warning"

}
-----------------------------------
Bunu kullanmak istediğimiz herhangi conponent.ts dosyasında servisi çağırarak kullanabiliriz. Şimdi Layout.component.ts dosyasında servis çağırarak kullanıyoruz.
---------------------------------
import { Component, OnInit } from '@angular/core';
import { AlertifyService, MessageType } from '../../services/admin/alertify.service';

@Component({
  selector: 'app-layout',
  standalone: false,
  
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent implements OnInit {
  constructor(private alertifyService:AlertifyService){}
  ngOnInit(): void {
 this.alertifyService.message("Başarılı",MessageType.Success)
  }

}
--------------------------------
pozisyonu için https://alertifyjs.com/notifier/position.html sayfasında  alertify.set('notifier','position', 'bottom-right'); şeklinde kullanılacağı söylenmektedir alertify.service dosyasında şu şekilde değişiklik yapıyoruz.
------------------------------
import { Injectable } from '@angular/core';
declare var alertify:any

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

  constructor() { }

  message(message:string,messageType:MessageType,position:Position){
    alertify.set('notifier','position', position);
    alertify[messageType](message)   
    
  }
}

export enum MessageType{
  Error="error",
  Message="message",
  Notify="notify",
  Success="success",
  Warning="warning"

}

export enum Position{
  TopCenter="top-center",
  TopRight="top-right",
  TopLeft="top-left",
  BottomCenter="bottom-center",
  BottomRight="bottom-right",
  BottomLeft="bottom-left"
}
-------------------------------
Layout.component.ts dosyasında şu şekilde kullanıyoruz.
----------------------------
import { Component, OnInit } from '@angular/core';
import { AlertifyService, MessageType, Position } from '../../services/admin/alertify.service';

@Component({
  selector: 'app-layout',
  standalone: false,
  
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent implements OnInit {
  constructor(private alertifyService:AlertifyService){}
  ngOnInit(): void {
 this.alertifyService.message("Başarılı",MessageType.Success,Position.TopCenter)
  }

}
-------------------------------
özelleştirmeye devam ediyoruz https://alertifyjs.com/notifier/delay.html sayfasında notificationun ne kadar süre kalacağı belirtilmekte ayrıca birde dismissAll özelliği var. Açılan tüm notification ların hepsini kapatır.
-----------------------------
import { Injectable } from '@angular/core';
declare var alertify:any

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

  constructor() { }

  message(message:string,messageType:MessageType,position:Position,delay:number){
    alertify.set('notifier','delay', delay);

    alertify.set('notifier','position', position);

    alertify[messageType](message)    
    
  }
  
  dismiss(){
    alertify.dismissAll();
  }
}

export enum MessageType{
  Error="error",
  Message="message",
  Notify="notify",
  Success="success",
  Warning="warning"

}

export enum Position{
  TopCenter="top-center",
  TopRight="top-right",
  TopLeft="top-left",
  BottomCenter="bottom-center",
  BottomRight="bottom-right",
  BottomLeft="bottom-left"
}
-------------------------------
Kullanımı için layout.component.ts dosyasından dashboard.component.ts dosyasına taşıyalım. ve testimizi orada yapalım dashboard.component.html sayfasında 2 buton ekliyoruz biri notification oluşturuyor diğeri ise oluşan tüm notificationları yok ediyor.
-------------------------------
<button (click)="m()">Alertify</button><button (click)="dismiss()">Dismiss</button>
-----------------------------
import { Component, OnInit } from '@angular/core';
import { AlertifyService, MessageType, Position } from '../../../../services/admin/alertify.service';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {

  constructor(private alertifyService:AlertifyService){}
  ngOnInit(): void {
    
  }

  m(){
    this.alertifyService.message("Başarılı",MessageType.Success,Position.TopCenter,5)

  }
  dismiss(){
    this.alertifyService.dismiss()
  }

}
----------------------------
son bir özellik ekliyoruz. bu da sadece 1 tane notificationa izin veriyor diğerlerine izin vermiyor. bu özellik te dismissother özelliği ve servisimizin son hali şu şekilde oluyor.
-----------------------------
import { Injectable } from '@angular/core';
declare var alertify:any

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

  constructor() { }

  message(message:string,messageType:MessageType,position:Position,delay:number,dismissOthers:boolean=false){
    alertify.set('notifier','delay', delay);

    alertify.set('notifier','position', position);

    const msj=alertify[messageType](message) 
    if(dismissOthers)
      msj.dismissOthers();   
    
  }
  
  dismiss(){
    alertify.dismissAll();
  }
}

export enum MessageType{
  Error="error",
  Message="message",
  Notify="notify",
  Success="success",
  Warning="warning"

}

export enum Position{
  TopCenter="top-center",
  TopRight="top-right",
  TopLeft="top-left",
  BottomCenter="bottom-center",
  BottomRight="bottom-right",
  BottomLeft="bottom-left"
}
-----------------------------
kullanımıda şu şekilde oluyor.
-----------------------------
import { Component, OnInit } from '@angular/core';
import { AlertifyService, MessageType, Position } from '../../../../services/admin/alertify.service';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {

  constructor(private alertifyService:AlertifyService){}
  ngOnInit(): void {
    
  }

  m(){
    this.alertifyService.message("Başarılı",MessageType.Success,Position.TopCenter,5,true)

  }
  dismiss(){
    this.alertifyService.dismiss()
  }

}
------------------------------
Böylece alertify özelleştirmemiz bitti.artık dashboard.component.ts dosyasını temizleyebiliriz. alertify serviste böyle parametre olarak çok kullanışlı değil o yüzden bu parametreleri bir nesneye çevirerek kullanacağız.
--------------------------
import { Injectable } from '@angular/core';
declare var alertify:any

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

  constructor() { }

  //message(message:string,messageType:MessageType,position:Position,delay:number,dismissOthers:boolean=false)
  
  message(message:string,options:Partial<AlertifyOptions>) { 
  alertify.set('notifier','delay', options.delay);
    alertify.set('notifier','position', options.position);
    const msj=alertify[options.messageType](message) 
    if(options.dismissOthers)
      msj.dismissOthers();   
    
  } 
  
  dismiss(){
    alertify.dismissAll();
  }
}
export class AlertifyOptions{
    messageType:MessageType=MessageType.Message
    position:Position=Position.BottomRight
    delay:number=3
    dismissOthers:boolean=false
}

export enum MessageType{
  Error="error",
  Message="message",
  Notify="notify",
  Success="success",
  Warning="warning"

}

export enum Position{
  TopCenter="top-center",
  TopRight="top-right",
  TopLeft="top-left",
  BottomCenter="bottom-center",
  BottomRight="bottom-right",
  BottomLeft="bottom-left"
}

--------------------------
<button (click)="m()">Alertify</button><button (click)="dismiss()">Dismiss</button>
-------------------------
import { Component, OnInit } from '@angular/core';
import { AlertifyService, MessageType, Position } from '../../../../services/admin/alertify.service';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {

  constructor(private alertifyService:AlertifyService){}
  ngOnInit(): void {
    
  }

  m(){
    this.alertifyService.message("Başarılı",{
      messageType:MessageType.Warning,
      delay:5,
      position:Position.BottomRight,
      dismissOthers:true
    })

  }
  dismiss(){
    this.alertifyService.dismiss()
  }

}
----------------------------
alertifi özelliklerini nesneye çevirerek kullanıyoruz.Şimdi UI kısmında kullanmak üzere ngx-toastr kütüphanesini kurup kendimize göre özelleştireceğiz. öncelikle npm install ngx-toastr --save komutuyla kütüphaneyi yüklüyoruz. npm install @angular/animations --save komutuyla animasyon kütüphanesini de yüklüyoruz. Bunu da kendimize göre düzenleyeceğimizden bir customToastrService oluşturuyoruz.
-------------------------------
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class CustomToastrService {

  constructor(private toastrService:ToastrService) { }

  message(message:string, title:string,toastrOptions:Partial<ToastrOptions>){
    this.toastrService[toastrOptions.toastrmessageType](message,title,{
      positionClass:toastrOptions.toastrPosition,
      messageClass:toastrOptions.toastrmessageType
    })
  }
}

export enum ToastrMessageType{
Success="success",
Info="info",
Warning="warning",
Error="error"
}

export enum ToastrPosition{
TopRight="toast-top-right",
BottomRight="toast-bottom-right",
BottomLeft="toast-bottom-left",
TopLeft="toast-top-left",
TopFullWidth="toast-top-full-width",
BottomFullWidth="toast-bottom-full-width",
TopCenter="toast-top-center",
BottomCenter="toast-bottom-center"

}

export class ToastrOptions{
  toastrmessageType:ToastrMessageType=ToastrMessageType.Success
  toastrPosition:ToastrPosition
}
--------------------------------
Bunu test kullanımımız olan dashboard.component.ts de şu şekilde kullanıyoruz. Burayı şimdilik test olarak kullanıyoruz burayı daha sonra farklı şekilde düzenleyeceğiz.
------------------------------
import { Component, OnInit } from '@angular/core';
import { AlertifyService, MessageType, Position } from '../../../../services/admin/alertify.service';
import { CustomToastrService, ToastrMessageType, ToastrPosition } from '../../../../services/ui/custom-toastr.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {

  constructor(private alertifyService:AlertifyService,private customToastrService:CustomToastrService){}
  ngOnInit(): void {
    
  }

  m(){
    this.alertifyService.message("Başarılı",{
      messageType:MessageType.Error,
      delay:5,
      position:Position.BottomRight,
      dismissOthers:false
    })
    this.customToastrService.message("Sipariş Başarılı","Sipariş",{
      toastrmessageType:ToastrMessageType.Error,
      toastrPosition:ToastrPosition.BottomCenter
    })
  }
  dismiss(){
    this.alertifyService.dismiss()
  }

}
-------------------------------
Bu düzenleme bizim için yeterli diğer özelliklerini kullanmaya gerek yok. Böylece toastr kütüphanemiz tamamlandı şimdi sayfalar arası geçişte bekleniyor...,loading gibi bir şey yazan bir animasyon kütüphenesi olan ngx-spinner kütüphanesini yükleyeceğiz ve kendimize göre özelleştireceğiz. Hangi animasyonları kullanacaksak onların css dosyalarını angular.json dosyasıntaki styles kısmına ekliyoruz. Hangi component te kullanacaksak o componentin bağlı olduğu modüle spinner modül eklenir.
---------------------------------
Angular.json
--------------------------
"node_modules/ngx-spinner/animations/ball-scale-multiple.css",
"node_modules/ngx-spinner/animations/ball-atom.css",
"node_modules/ngx-spinner/animations/ball-spin-fade-rotating.css"
--------------------------------
app.component.html kullanacaksak app.module eklenir.
--------------------------------

<nav class="navbar navbar-expand-lg bg-body-tertiary">
    <div class="container-fluid">
      <a class="navbar-brand" routerLink="">Mini E-Ticaret</a>
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarSupportedContent">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <li class="nav-item">
            <a class="nav-link active" aria-current="page" routerLink="">Home</a>
          </li>
          <li class="nav-item">
            <a class="nav-link active" aria-current="page" routerLink="products">Products</a>
          </li>
          <li class="nav-item">
            <a class="nav-link active" aria-current="page" routerLink="basket">Basket</a>
          </li>
          <li class="nav-item">
            <a class="nav-link active" aria-current="page" routerLink="admin">Yönetim Paneli</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" href="#">Link</a>
          </li>
          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
              Dropdown
            </a>
            <ul class="dropdown-menu">
              <li><a class="dropdown-item" href="#">Action</a></li>
              <li><a class="dropdown-item" href="#">Another action</a></li>
              <li><hr class="dropdown-divider"></li>
              <li><a class="dropdown-item" href="#">Something else here</a></li>
            </ul>
          </li>
          <li class="nav-item">
            <a class="nav-link disabled" aria-disabled="true">Disabled</a>
          </li>
        </ul>
        <form class="d-flex" role="search">
          <input class="form-control me-2" type="search" placeholder="Search" aria-label="Search">
          <button class="btn btn-outline-success" type="submit">Search</button>
        </form>
      </div>
    </div>
  </nav>

  


  <router-outlet></router-outlet>
  <ngx-spinner type="ball-scale-multiple"></ngx-spinner>
  -----------------------------
  import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AdminModule } from './admin/admin.module';
import { UiModule } from './ui/ui.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {ToastrModule} from "ngx-toastr"
import {NgxSpinnerModule} from "ngx-spinner"

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    NgxSpinnerModule,
    ToastrModule.forRoot(),
    AppRoutingModule,
    AdminModule,
    UiModule
  ],
 
  providers: [
    provideClientHydration(withEventReplay()),
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
---------------------------------
Çalıştığını gördük şimdi bunu kendimize uygun şekilde düzenleyelim. ng g c base ile bir base component oluşturuyoruz. Ama bu base de selector,html ve css kullanılmayacak hatta bunun component ifadesinin olmasına bile gerek yok adı bir component ama kullanımı class olacak. implements OnInit yapılanmasını da siliyoruz böylece bir class kalıyor.Madem kullanmayacağız o zaman base.component.html ve base.component.css ve test için kullanılan spec dosyasını siliyoruz. sadece geriye base.component.ts kalıyor. Burada bir metot oluşturuyoruz. bu bütün componentlerde kullanılacak. kullanacağımız spinnerlere isim vererek html taglarini düzenliyoruz. Routing işlemlerinde kullanacağımız componentleri de base.component.ts den türeteceğiz.Bu componentler customers,dashboard,orders,products

Base.component
----------------------------
import { NgxSpinnerService } from "ngx-spinner";
import { timeout } from "rxjs";


export class BaseComponent  {
constructor(private spinner:NgxSpinnerService){}

showSpinner(spinnerNameType:SpinnerType){
this.spinner.show(spinnerNameType)

setTimeout(()=>this.hideSpinner(spinnerNameType),3000)
}

hideSpinner(spinnerNameType:SpinnerType){
  this.spinner.hide(spinnerNameType)
}

}

export enum SpinnerType{
  BallScaleMultiple="s1",
  BallAtom="s2",
  BallSpinClockwiseFadeRotating="s3"
}
----------------------------------
Türeyen componentler şu şekilde olacak örneğin customer.component
--------------------------------
import { Component, OnInit } from '@angular/core';
import { BaseComponent, SpinnerType } from '../../../../base/base.component';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-customers',
  standalone: false,
  
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css'
})
export class CustomersComponent extends BaseComponent implements OnInit {
  constructor(spinner:NgxSpinnerService){
    super(spinner)
  }
  ngOnInit(): void {
    this.showSpinner(SpinnerType.BallAtom)
  }

}
-------------------------------




 
  




