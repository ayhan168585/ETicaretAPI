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
Böylece dummy data ile çalışsakta onion architecture sistemi doğru kurulduğundan dolayı ürünler getirilebildi.





