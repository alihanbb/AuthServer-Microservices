# .NET 9 Mikroservis Mimarisi

Bu proje, .NET 9 kullanarak geliþtirilmiþ modern mikroservis mimarisini gösterir. Clean Architecture prensipleri, CQRS pattern ve Event-Driven Architecture kullanarak ölçeklenebilir bir sistem sunar.

## ??? Mimari Genel Bakýþ

### Mikroservisler
- **Auth Server API** - Kimlik doðrulama ve yetkilendirme servisi
- **Customer API** - Müþteri yönetimi servisi  
- **Product API** - Ürün katalog servisi
- **Order API** - Sipariþ yönetimi servisi

### Teknoloji Stack
- **.NET 9** - Ana framework
- **PostgreSQL** - Veritabaný
- **RabbitMQ** - Mesaj kuyruðu ve event handling
- **Docker & Docker Compose** - Containerization
- **Entity Framework Core 9** - ORM
- **JWT Bearer Authentication** - Kimlik doðrulama
- **AutoMapper** - Object mapping
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **Scalar** - API documentation

## ?? Özellikler

### Kimlik Doðrulama & Yetkilendirme
- JWT tabanlý authentication
- Refresh token desteði
- Role-based authorization
- CRUD tabanlý izin sistemi

### Mikroservis Mimarisi
- Her servis kendi veritabanýna sahip
- Event-driven communication
- Clean Architecture pattern
- CQRS ile komut/sorgu ayrýmý

### Event-Driven Architecture
- Domain events ile loosely coupled communication
- RabbitMQ ile async messaging
- Event sourcing pattern

### Containerization
- Docker containerization
- Docker Compose ile orchestration
- PostgreSQL ve RabbitMQ container desteði
- PgAdmin web interface

## ??? Kurulum

### Gereksinimler
- .NET 9 SDK
- Docker Desktop
- Git

### Adým 1: Repository'yi klonlayýn
```bash
git clone https://github.com/[username]/dotnet-microservices-architecture.git
cd dotnet-microservices-architecture
```

### Adým 2: Docker ile servisleri baþlatýn
```bash
docker-compose up -d
```

### Adým 3: Veritabaný migration'larýný çalýþtýrýn
```bash
# Her mikroservis için migration çalýþtýrýn
dotnet ef database update --project AuhtServer.Api
dotnet ef database update --project Customer.Api
dotnet ef database update --project Product.Api
dotnet ef database update --project Order.Api
```

## ?? API Endpoints

### Auth Server (Port: 8080/8081)
- `POST /api/auth/login` - Kullanýcý giriþi
- `POST /api/auth/register` - Kullanýcý kaydý
- `POST /api/auth/refresh` - Token yenileme
- `POST /api/role/create` - Rol oluþturma
- `POST /api/role/assign` - Kullanýcýya rol atama
- `POST /api/role/remove` - Kullanýcýdan rol kaldýrma

### Customer API (Port: 8082/8083)
- `GET /api/customer` - Müþteri listesi
- `POST /api/customer` - Müþteri oluþturma
- `PUT /api/customer/{id}` - Müþteri güncelleme
- `DELETE /api/customer/{id}` - Müþteri silme

### Product API (Port: 8086/8087)
- `GET /api/product` - Ürün listesi
- `POST /api/product` - Ürün oluþturma
- `PUT /api/product/{id}` - Ürün güncelleme
- `DELETE /api/product/{id}` - Ürün silme

### Order API (Port: 8084/8085)
- `GET /api/order` - Sipariþ listesi
- `POST /api/order` - Sipariþ oluþturma
- `PUT /api/order/{id}` - Sipariþ güncelleme
- `DELETE /api/order/{id}` - Sipariþ silme

## ?? Veritabaný Yönetimi

PgAdmin web arayüzüne http://localhost:5050 adresinden eriþebilirsiniz:
- **Email:** admin@authserver.com
- **Password:** admin123

### Veritabaný Baðlantý Bilgileri
- **Auth Server DB:** localhost:5433
- **Customer DB:** localhost:5435
- **Product DB:** localhost:5434
- **Order DB:** localhost:5436

## ?? Geliþtirme

### Proje Yapýsý
```
??? AuhtServer.Api/              # Auth mikroservisi
?   ??? Controllers/             # API controllers
?   ??? Dockerfile              # Docker configuration
?   ??? AuhtServer.Api.csproj   # Project file
??? Authserver.Application/      # Auth application layer
??? Authserver.Infrastructure/   # Auth infrastructure layer
??? AuthServerDomain/           # Auth domain layer
??? Customer.Api/               # Customer mikroservisi
??? Customer.Application/       # Customer application layer
??? Customer.Infrastructure/    # Customer infrastructure layer
??? Customer.Domain/           # Customer domain layer
??? Product.Api/               # Product mikroservisi
??? Product.Application/       # Product application layer
??? Product.Infrastructure/    # Product infrastructure layer
??? Product.Domain/           # Product domain layer
??? Order.Api/                # Order mikroservisi
??? Order.Application/        # Order application layer
??? Order.Infrastructures/    # Order infrastructure layer
??? Order.Domain/            # Order domain layer
??? SharedLibrary/           # Ortak kütüphaneler
??? docker-compose.yml       # Docker orchestration
??? README.md               # Bu dosya
```

### Clean Architecture Katmanlarý
- **API Layer** - Controllers ve endpoints
- **Application Layer** - Business logic, CQRS handlers ve MediatR
- **Infrastructure Layer** - Data access ve external services
- **Domain Layer** - Entities ve domain logic

### Shared Library
- **Common** - Ortak base sýnýflar ve response modelleri
- **Events** - Domain events ve event handling
- **Messaging** - RabbitMQ ile event publishing

## ?? Docker Servisleri

| Servis | Port | Açýklama |
|--------|------|----------|
| Auth Server | 8080, 8081 | Kimlik doðrulama API |
| Customer API | 8082, 8083 | Müþteri yönetimi API |
| Product API | 8086, 8087 | Ürün katalog API |
| Order API | 8084, 8085 | Sipariþ yönetimi API |
| RabbitMQ | 5672, 15672 | Mesaj kuyruðu ve management UI |
| PgAdmin | 5050 | Veritabaný yönetimi |

## ?? Kimlik Doðrulama

### JWT Configuration
Projede JWT authentication kullanýlmaktadýr. Aþaðýdaki environment variables'lar gereklidir:

```
Jwt__Key=supersecretkeythisstrong12345678
Jwt__Issuer=AuthServer
Jwt__Audience=AuthClients
```

### Roller ve Ýzinler
- **Admin** - Tam sistem eriþimi
- **User** - Temel kullanýcý iþlemleri
- CRUD tabanlý izin sistemi (create, read, update, delete)

## ?? Event-Driven Architecture

### Events
- **CustomerCreatedEvent** - Müþteri oluþturulduðunda
- **CustomerDeletedEvent** - Müþteri silindiðinde
- **ProductCreatedEvent** - Ürün oluþturulduðunda
- **ProductDeletedEvent** - Ürün silindiðinde

### RabbitMQ Configuration
```
RabbitMQ__Host=rabbitmq
RabbitMQ__Port=5672
RabbitMQ__Username=rabbitmq
RabbitMQ__Password=rabbitmq123
```

## ?? Testing

### Unit Tests Çalýþtýrma
```bash
dotnet test
```

### API Testing
Scalar UI ile API'leri test edebilirsiniz:
- Auth Server: http://localhost:8080/scalar/v1
- Customer API: http://localhost:8082/scalar/v1
- Product API: http://localhost:8086/scalar/v1
- Order API: http://localhost:8084/scalar/v1

## ?? Lisans

Bu proje MIT lisansý ile lisanslanmýþtýr.

## ?? Katkýda Bulunma

1. Fork edin
2. Feature branch oluþturun (`git checkout -b feature/amazing-feature`)
3. Deðiþikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluþturun

## ?? Ýletiþim

Proje hakkýnda sorularýnýz için issue açabilirsiniz.

## ?? Troubleshooting

### Yaygýn Sorunlar

1. **Docker container'larý baþlamýyor**
   ```bash
   docker-compose down
   docker-compose up -d --build
   ```

2. **Veritabaný baðlantý hatasý**
   - PostgreSQL container'larýnýn ayakta olduðunu kontrol edin
   - Connection string'leri doðrulayýn

3. **JWT token hatasý**
   - JWT ayarlarýnýn doðru yapýlandýrýldýðýný kontrol edin
   - Token'ýn expire olmadýðýndan emin olun

4. **RabbitMQ baðlantý hatasý**
   - RabbitMQ container'ýnýn ayakta olduðunu kontrol edin
   - Management UI'dan kuyruklarý kontrol edin: http://localhost:15672