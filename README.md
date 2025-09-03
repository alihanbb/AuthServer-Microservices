# .NET 9 Mikroservis Mimarisi

Bu proje, .NET 9 kullanarak gelistirilmis modern mikroservis mimarisini gosterir. Clean Architecture prensipleri, CQRS pattern ve Event-Driven Architecture kullanarak olceklenebilir bir sistem sunar.

## ??? Mimari Genel Bakis

### Mikroservisler
- **Auth Server API** - Kimlik dogrulama ve yetkilendirme servisi
- **Customer API** - Musteri yonetimi servisi  
- **Product API** - Urun katalog servisi
- **Order API** - Siparis yonetimi servisi

### Teknoloji Stack
- **.NET 9** - Ana framework
- **PostgreSQL** - Veritabani
- **RabbitMQ** - Mesaj kuyrugu ve event handling
- **Docker & Docker Compose** - Containerization
- **Entity Framework Core 9** - ORM
- **JWT Bearer Authentication** - Kimlik dogrulama
- **AutoMapper** - Object mapping
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **Scalar** - API documentation

## ?? Ozellikler

### Kimlik Dogrulama & Yetkilendirme
- JWT tabanli authentication
- Refresh token destegi
- Role-based authorization
- CRUD tabanli izin sistemi

### Mikroservis Mimarisi
- Her servis kendi veritabanina sahip
- Event-driven communication
- Clean Architecture pattern
- CQRS ile komut/sorgu ayrimi

### Event-Driven Architecture
- Domain events ile loosely coupled communication
- RabbitMQ ile async messaging
- Event sourcing pattern

### Containerization
- Docker containerization
- Docker Compose ile orchestration
- PostgreSQL ve RabbitMQ container destegi
- PgAdmin web interface

## ??? Kurulum

### Gereksinimler
- .NET 9 SDK
- Docker Desktop
- Git

### Adim 1: Repository'yi klonlayin git clone https://github.com/[username]/dotnet-microservices-architecture.git
cd dotnet-microservices-architecture
### Adim 2: Docker ile servisleri baslatindocker-compose up -d
### Adim 3: Veritabani migration'larini calistirin# Her mikroservis icin migration calistirin
dotnet ef database update --project AuhtServer.Api
dotnet ef database update --project Customer.Api
dotnet ef database update --project Product.Api
dotnet ef database update --project Order.Api
## ?? API Endpoints

### Auth Server (Port: 8080/8081)
- `POST /api/auth/login` - Kullanici girisi
- `POST /api/auth/register` - Kullanici kaydi
- `POST /api/auth/refresh` - Token yenileme
- `POST /api/role/create` - Rol olusturma
- `POST /api/role/assign` - Kullaniciya rol atama
- `POST /api/role/remove` - Kullanicidan rol kaldirma

### Customer API (Port: 8082/8083)
- `GET /api/customer` - Musteri listesi
- `POST /api/customer` - Musteri olusturma
- `PUT /api/customer/{id}` - Musteri guncelleme
- `DELETE /api/customer/{id}` - Musteri silme

### Product API (Port: 8086/8087)
- `GET /api/product` - Urun listesi
- `POST /api/product` - Urun olusturma
- `PUT /api/product/{id}` - Urun guncelleme
- `DELETE /api/product/{id}` - Urun silme

### Order API (Port: 8084/8085)
- `GET /api/order` - Siparis listesi
- `POST /api/order` - Siparis olusturma
- `PUT /api/order/{id}` - Siparis guncelleme
- `DELETE /api/order/{id}` - Siparis silme

## ?? Veritabani Yonetimi

PgAdmin web arayuzune http://localhost:5050 adresinden erisebilirsiniz:
- **Email:** admin@authserver.com
- **Password:** admin123

### Veritabani Baglanti Bilgileri
- **Auth Server DB:** localhost:5433
- **Customer DB:** localhost:5435
- **Product DB:** localhost:5434
- **Order DB:** localhost:5436

## ?? Gelistirme

### Proje Yapisi
??? AuhtServer.Api/             
?   ??? Controllers/           
?   ??? Dockerfile             
?   ??? AuhtServer.Api.csproj 
??? Authserver.Application/      
??? Authserver.Infrastructure/  
??? AuthServerDomain/           
??? Customer.Api/               
??? Customer.Application/       
??? Customer.Infrastructure/   
??? Customer.Domain/           
??? Product.Api/               
??? Product.Application/       
??? Product.Infrastructure/    
??? Product.Domain/           
??? Order.Api/               
??? Order.Application/        
??? Order.Infrastructures/    
??? Order.Domain/         
??? SharedLibrary/          
??? docker-compose.yml       
??? README.md               
### Clean Architecture Katmanlari
- **API Layer** - Controllers ve endpoints
- **Application Layer** - Business logic, CQRS handlers ve MediatR
- **Infrastructure Layer** - Data access ve external services
- **Domain Layer** - Entities ve domain logic

### Shared Library
- **Common** - Ortak base siniflar ve response modelleri
- **Events** - Domain events ve event handling
- **Messaging** - RabbitMQ ile event publishing

## ?? Docker Servisleri

| Servis | Port | Aciklama |
|--------|------|----------|
| Auth Server | 8080, 8081 | Kimlik dogrulama API |
| Customer API | 8082, 8083 | Musteri yonetimi API |
| Product API | 8086, 8087 | Urun katalog API |
| Order API | 8084, 8085 | Siparis yonetimi API |
| RabbitMQ | 5672, 15672 | Mesaj kuyrugu ve management UI |
| PgAdmin | 5050 | Veritabani yonetimi |

## ?? Kimlik Dogrulama

### JWT Configuration
Projede JWT authentication kullanilmaktadir. Asagidaki environment variables'lar gereklidir:
Jwt__Key=supersecretkeythisstrong12345678
Jwt__Issuer=AuthServer
Jwt__Audience=AuthClients
### Roller ve Izinler
- **Admin** - Tam sistem erisimi
- **User** - Temel kullanici islemleri
- CRUD tabanli izin sistemi (create, read, update, delete)

## ?? Event-Driven Architecture

### Events
- **CustomerCreatedEvent** - Musteri olusturuldugunda
- **CustomerDeletedEvent** - Musteri silindiginde
- **ProductCreatedEvent** - Urun olusturuldugunda
- **ProductDeletedEvent** - Urun silindiginde

### RabbitMQ ConfigurationRabbitMQ__Host=rabbitmq
RabbitMQ__Port=5672
RabbitMQ__Username=rabbitmq
RabbitMQ__Password=rabbitmq123
## ?? Testing

### Unit Tests Calistirmadotnet test
### API Testing
Scalar UI ile API'leri test edebilirsiniz:
- Auth Server: http://localhost:8080/scalar/v1
- Customer API: http://localhost:8082/scalar/v1
- Product API: http://localhost:8086/scalar/v1
- Order API: http://localhost:8084/scalar/v1

## ?? Lisans

Bu proje MIT lisansi ile lisanslanmistir.

## ?? Katkida Bulunma

1. Fork edin
2. Feature branch olusturun (`git checkout -b feature/amazing-feature`)
3. Degisikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request olusturun

## ?? Iletisim

Proje hakkinda sorulariniz icin issue acabilirsiniz.

## ?? Troubleshooting

### Yaygin Sorunlar

1. **Docker container'lari baslamiyor**docker-compose down
docker-compose up -d --build
2. **Veritabani baglanti hatasi**
   - PostgreSQL container'larinin ayakta oldugunu kontrol edin
   - Connection string'leri dogrulayin

3. **JWT token hatasi**
   - JWT ayarlarinin dogru yapilandirildigini kontrol edin
   - Token'in expire olmadigýndan emin olun

4. **RabbitMQ baglanti hatasi**
   - RabbitMQ container'inin ayakta oldugunu kontrol edin
   - Management UI'dan kuyruklari kontrol edin: http://localhost:15672