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

2. Domain Katmanı
Order entity'si ve iş kuralları yazıldı.
İş kuralları merkezi bir yerde olduğundan kod değişse bile kurallar korunur.

3. Application Katmanı (CQRS)
Command (yazma) ve Query (okuma) işlemleri ayrıldı. Bu sayede okuma ve yazma operasyonları farklı optimize edilebilir.

4. Infrastructure Katmanı
Database, RabbitMQ, Redis bağlantılarını implement ettik. Dependency'ler değişse bile sadece bu katman değişecek iş akışı etkilenemeyecek.

5. Web API Katmanı
REST endpoint, JWT authentication ve Swagger UI kullanıldı. Bu sayede daha güvenli bir sistem oldu.

6. Worker Service
Background'da sürekli çalışan sipariş işleme servisi oldugundan API hızlı response verecek.

7. Security & Validation
JWT token'lar sayesinde sadece yetkili kullanıcılar erişebilir ve input validation kuralları sayesinde hatalı verilerin sisteme girilmesi önlendi.

8. Logging & Monitoring
Tüm işlemleri log'layan Serilog yapısı kuruldu. Hata durumunda debug edilebilir ve sistem performansı izlenebilsin diye

9. Modern Patterns & Practices
Kod güncellenebilir, geliştirilebilir olsun diye SOLID prensipleri, Repository pattern kullanıldı. Yüksek performans elde edilsin diye Async/await.

10. Testing & Documentation
Swagger UI kurulumuyla yapılan işler ve api servisleri test edilebilir oldu.

11. Production-Ready Features
Production ortamında stabil çalışsın, hata durumlarında sistem çökmesin diye Configuration management, error handling yapıldı.
