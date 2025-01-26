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
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
--------------------------------



