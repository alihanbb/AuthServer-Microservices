# .NET 9 Mikroservis Mimarisi

Bu proje, .NET 9 kullanarak geli�tirilmi� modern mikroservis mimarisini g�sterir. Clean Architecture prensipleri, CQRS pattern ve Event-Driven Architecture kullanarak �l�eklenebilir bir sistem sunar.

## ??? Mimari Genel Bak��

### Mikroservisler
- **Auth Server API** - Kimlik do�rulama ve yetkilendirme servisi
- **Customer API** - M��teri y�netimi servisi  
- **Product API** - �r�n katalog servisi
- **Order API** - Sipari� y�netimi servisi

### Teknoloji Stack
- **.NET 9** - Ana framework
- **PostgreSQL** - Veritaban�
- **RabbitMQ** - Mesaj kuyru�u ve event handling
- **Docker & Docker Compose** - Containerization
- **Entity Framework Core 9** - ORM
- **JWT Bearer Authentication** - Kimlik do�rulama
- **AutoMapper** - Object mapping
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **Scalar** - API documentation

## ?? �zellikler

### Kimlik Do�rulama & Yetkilendirme
- JWT tabanl� authentication
- Refresh token deste�i
- Role-based authorization
- CRUD tabanl� izin sistemi

### Mikroservis Mimarisi
- Her servis kendi veritaban�na sahip
- Event-driven communication
- Clean Architecture pattern
- CQRS ile komut/sorgu ayr�m�

### Event-Driven Architecture
- Domain events ile loosely coupled communication
- RabbitMQ ile async messaging
- Event sourcing pattern

### Containerization
- Docker containerization
- Docker Compose ile orchestration
- PostgreSQL ve RabbitMQ container deste�i
- PgAdmin web interface

## ??? Kurulum

### Gereksinimler
- .NET 9 SDK
- Docker Desktop
- Git

### Ad�m 1: Repository'yi klonlay�n
```bash
git clone https://github.com/[username]/dotnet-microservices-architecture.git
cd dotnet-microservices-architecture
```

### Ad�m 2: Docker ile servisleri ba�lat�n
```bash
docker-compose up -d
```

### Ad�m 3: Veritaban� migration'lar�n� �al��t�r�n
```bash
# Her mikroservis i�in migration �al��t�r�n
dotnet ef database update --project AuhtServer.Api
dotnet ef database update --project Customer.Api
dotnet ef database update --project Product.Api
dotnet ef database update --project Order.Api
```

## ?? API Endpoints

### Auth Server (Port: 8080/8081)
- `POST /api/auth/login` - Kullan�c� giri�i
- `POST /api/auth/register` - Kullan�c� kayd�
- `POST /api/auth/refresh` - Token yenileme
- `POST /api/role/create` - Rol olu�turma
- `POST /api/role/assign` - Kullan�c�ya rol atama
- `POST /api/role/remove` - Kullan�c�dan rol kald�rma

### Customer API (Port: 8082/8083)
- `GET /api/customer` - M��teri listesi
- `POST /api/customer` - M��teri olu�turma
- `PUT /api/customer/{id}` - M��teri g�ncelleme
- `DELETE /api/customer/{id}` - M��teri silme

### Product API (Port: 8086/8087)
- `GET /api/product` - �r�n listesi
- `POST /api/product` - �r�n olu�turma
- `PUT /api/product/{id}` - �r�n g�ncelleme
- `DELETE /api/product/{id}` - �r�n silme

### Order API (Port: 8084/8085)
- `GET /api/order` - Sipari� listesi
- `POST /api/order` - Sipari� olu�turma
- `PUT /api/order/{id}` - Sipari� g�ncelleme
- `DELETE /api/order/{id}` - Sipari� silme

## ?? Veritaban� Y�netimi

PgAdmin web aray�z�ne http://localhost:5050 adresinden eri�ebilirsiniz:
- **Email:** admin@authserver.com
- **Password:** admin123

### Veritaban� Ba�lant� Bilgileri
- **Auth Server DB:** localhost:5433
- **Customer DB:** localhost:5435
- **Product DB:** localhost:5434
- **Order DB:** localhost:5436

## ?? Geli�tirme

### Proje Yap�s�
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
??? SharedLibrary/           # Ortak k�t�phaneler
??? docker-compose.yml       # Docker orchestration
??? README.md               # Bu dosya
```

### Clean Architecture Katmanlar�
- **API Layer** - Controllers ve endpoints
- **Application Layer** - Business logic, CQRS handlers ve MediatR
- **Infrastructure Layer** - Data access ve external services
- **Domain Layer** - Entities ve domain logic

### Shared Library
- **Common** - Ortak base s�n�flar ve response modelleri
- **Events** - Domain events ve event handling
- **Messaging** - RabbitMQ ile event publishing

## ?? Docker Servisleri

| Servis | Port | A��klama |
|--------|------|----------|
| Auth Server | 8080, 8081 | Kimlik do�rulama API |
| Customer API | 8082, 8083 | M��teri y�netimi API |
| Product API | 8086, 8087 | �r�n katalog API |
| Order API | 8084, 8085 | Sipari� y�netimi API |
| RabbitMQ | 5672, 15672 | Mesaj kuyru�u ve management UI |
| PgAdmin | 5050 | Veritaban� y�netimi |

## ?? Kimlik Do�rulama

### JWT Configuration
Projede JWT authentication kullan�lmaktad�r. A�a��daki environment variables'lar gereklidir:

```
Jwt__Key=supersecretkeythisstrong12345678
Jwt__Issuer=AuthServer
Jwt__Audience=AuthClients
```

### Roller ve �zinler
- **Admin** - Tam sistem eri�imi
- **User** - Temel kullan�c� i�lemleri
- CRUD tabanl� izin sistemi (create, read, update, delete)

## ?? Event-Driven Architecture

### Events
- **CustomerCreatedEvent** - M��teri olu�turuldu�unda
- **CustomerDeletedEvent** - M��teri silindi�inde
- **ProductCreatedEvent** - �r�n olu�turuldu�unda
- **ProductDeletedEvent** - �r�n silindi�inde

### RabbitMQ Configuration
```
RabbitMQ__Host=rabbitmq
RabbitMQ__Port=5672
RabbitMQ__Username=rabbitmq
RabbitMQ__Password=rabbitmq123
```

## ?? Testing

### Unit Tests �al��t�rma
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

Bu proje MIT lisans� ile lisanslanm��t�r.

## ?? Katk�da Bulunma

1. Fork edin
2. Feature branch olu�turun (`git checkout -b feature/amazing-feature`)
3. De�i�ikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request olu�turun

## ?? �leti�im

Proje hakk�nda sorular�n�z i�in issue a�abilirsiniz.

## ?? Troubleshooting

### Yayg�n Sorunlar

1. **Docker container'lar� ba�lam�yor**
   ```bash
   docker-compose down
   docker-compose up -d --build
   ```

2. **Veritaban� ba�lant� hatas�**
   - PostgreSQL container'lar�n�n ayakta oldu�unu kontrol edin
   - Connection string'leri do�rulay�n

3. **JWT token hatas�**
   - JWT ayarlar�n�n do�ru yap�land�r�ld���n� kontrol edin
   - Token'�n expire olmad���ndan emin olun

4. **RabbitMQ ba�lant� hatas�**
   - RabbitMQ container'�n�n ayakta oldu�unu kontrol edin
   - Management UI'dan kuyruklar� kontrol edin: http://localhost:15672