Bu proje, modern .NET 8 teknolojileri kullanarak geliştirilmiş bir e-commerce sipariş yönetim sistemidir.
Clean Architecture prensipleri, CQRS pattern, ve mikroservis mimarisi yaklaşımları kullanılarak tasarlanmıştır.

Web API (REST APIs) ◄──► Worker Service (Background Processing) ◄──► External Services (DB,Redis,MQ) 

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

//------------------------------------------------------------------------------------------

1-PROJEYİ İNDİRİN-
git clone https://github.com/yourusername/ECommerceOrderSystem.git
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

//------------------------------------------------------------------------------------------
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


SWAGGER UI ile ilgili istekleri ile api isteklerini yaparak da test edebilirsiniz.

