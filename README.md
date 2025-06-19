# EcommerceOrderSystem
Bu proje, modern .NET 8 teknolojileri kullanarak geliştirilmiş bir e-commerce sipariş yönetim sistemidir. Clean Architecture prensipleri, CQRS pattern, ve mikroservis mimarisi yaklaşımları kullanılarak tasarlanmıştır. Web API (REST APIs) ◄──► Worker Service (Background Processing) ◄──► External Services (DB,Redis,MQ) 

Katman Yapısı
-Domain: Business entities ve kurallar
-Application: Use cases ve business logic (CQRS)
-Infrastructure: External service implementations
-API: REST endpoints ve controllers
-Worker: Background processing service

Kullanılan Teknolojiler
-.NET 8: Modern C# framework
-Entity Framework Core: ORM ve database operations
-MediatR: CQRS pattern implementation
-FluentValidation: Input validation
-Serilog: Structured logging
-JWT Bearer: Authentication
-Swagger: API documentation
-RabbitMQ: Message queuing (simulated)
-Redis: Caching layer (simulated)
-xUnit: Unit testing framework

Kurulum ve Çalıştırma
Ön Gereksinimler
-.NET 8 SDK
-Visual Studio 2022 veya VS Code
-Git

//-----------------------------------------------------------------------------------------------------------------

1-PROJEYİ İNDİRİN-
git clone https://github.com/hakanozdemir97/ECommerceOrderSystem.git
cd ECommerceOrderSystem

2-BAĞIMLILIKLARI YÜKLEYİN-
dotnet restore

3-PROJEYİ DERLEYİN-
dotnet build

4-API'yi ÇALIŞTIRIN-
cd src/ECommerceOrderSystem.API
dotnet run

5-Worker Service'i Çalıştırın (Yeni Terminal)
cd src/ECommerceOrderSystem.Worker
dotnet run

//--------------------------------------------------------------------------------------------------------------------
1. JWT Token Alma
Endpoint: POST /api/Auth/token
Request:
json{
  "userId": "user123",
  "password": "test123"
}
Response:
json{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2025-06-19T22:37:28Z",
  "userId": "user123"
}

2. Sipariş Oluşturma
Endpoint: POST /api/Orders
Authorization: Bearer {token} gerekli
Request:
json{
  "userId": "user123",
  "productId": "product456", 
  "quantity": 2,
  "paymentMethod": "CreditCard"
}
Response:
json{
  "orderId": "7f39ec48-d377-4c5d-b189-7bb81dddca4a",
  "status": "Pending",
  "createdAt": "2025-06-18T22:37:28Z"
}

3. Kullanıcı Siparişlerini Listeleme
Endpoint: GET /api/Orders/{userId}
Authorization: Bearer {token} gerekli
Response:
json[
  {
    "orderId": "7f39ec48-d377-4c5d-b189-7bb81dddca4a",
    "productId": "product456",
    "quantity": 2,
    "paymentMethod": "CreditCard",
    "status": "Pending", 
    "createdAt": "2025-06-18T22:37:28Z",
    "processedAt": null
  }
]

SWAGGER UI üzerinden Token alıp authentication yaptıktan sonra body kısmını değiştirip ilgili istekleri yapıp dönen responseları görüntüleyebilirsiniz...


//--------------------------------------------------------------------------------------------------------------------

1. API Layer (.NET Core Web API)
POST /api/Orders - Sipariş alma
GET /api/Orders/{userId} - Kullanıcı siparişleri
Input validation - FluentValidation
Error handling - Global middleware

2. Order Queueing (RabbitMQ)
Event publishing (mock implementation)
Database save (InMemory DB)
Cache invalidation

3. Order Processing Worker
Background service
Order processing
Redis logging
Notifications

4. Caching Layer (Redis)
2 dakika TTL
Cache invalidation
Performance optimization

5. Logging & Monitoring
Serilog
Console + File
Correlation ID

6. Security & Code Quality
JWT Authentication
Clean Architecture
SOLID Principles
Error handling

7. Bonus Points
Swagger UI
Unit test structure
Modern .NET 8

-----E-COMMERCE ORDER SYSTEM-----
1. Proje Yapısı (Clean Architecture)
Projeyi 5 katmana ayırdık (Domain, Application, Infrastructure, API, Worker)

3. Domain Katmanı
Order entity'si ve iş kuralları yazıldı.
İş kuralları merkezi bir yerde olduğundan kod değişse bile kurallar korunur.

4. Application Katmanı (CQRS)
Command (yazma) ve Query (okuma) işlemleri ayrıldı. Bu sayede okuma ve yazma operasyonları farklı optimize edilebilir.

5. Infrastructure Katmanı
Database, RabbitMQ, Redis bağlantılarını implement ettik. Dependency'ler değişse bile sadece bu katman değişecek iş akışı etkilenemeyecek.

6. Web API Katmanı
REST endpoint, JWT authentication ve Swagger UI kullanıldı. Bu sayede daha güvenli bir sistem oldu.

7. Worker Service
Background'da sürekli çalışan sipariş işleme servisi oldugundan API hızlı response verecek.

8. Security & Validation
JWT token'lar sayesinde sadece yetkili kullanıcılar erişebilir ve input validation kuralları sayesinde hatalı verilerin sisteme girilmesi önlendi.

9. Logging & Monitoring
Tüm işlemleri log'layan Serilog yapısı kuruldu. Hata durumunda debug edilebilir ve sistem performansı izlenebilsin diye

10. Modern Patterns & Practices
Kod güncellenebilir, geliştirilebilir olsun diye SOLID prensipleri, Repository pattern kullanıldı. Yüksek performans elde edilsin diye Async/await.

11. Testing & Documentation
Swagger UI kurulumuyla yapılan işler ve api servisleri test edilebilir oldu.

12. Production-Ready Features
Production ortamında stabil çalışsın, hata durumlarında sistem çökmesin diye Configuration management, error handling yapıldı.


