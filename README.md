# Yaz Okulu Ders Başvuru ve Kontenjan Takip Modülü

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Haliç Üniversitesi Yaz Okulu için geliştirilmiş, öğrencilerin derslere başvuru yapabildiği ve admin kullanıcıların bu başvuruları yönetebildiği modern bir web uygulaması.

## 📋 İçindekiler

- [Özellikler](#-özellikler)
- [Teknoloji Stack](#-teknoloji-stack)
- [Proje Mimarisi](#-proje-mimarisi)
- [Kurulum](#-kurulum)
- [Kullanım](#-kullanım)
- [API Dokümantasyonu](#-api-dokümantasyonu)
- [Docker](#-docker)
- [Ekran Görüntüleri](#-ekran-görüntüleri)

---

## 🚀 Özellikler

### Öğrenci Özellikleri
- ✅ GSM + SMS OTP ile giriş (kayıtsız giriş)
- ✅ Yaz okulu derslerini listeleme
- ✅ Kontenjan durumunu görüntüleme
- ✅ Derse başvuru yapma
- ✅ Başvuru durumunu takip etme
- ✅ Bir derse sadece 1 kez başvuru
- ✅ Dolu derslere başvuru engelleme

### Admin Özellikleri
- ✅ Kullanıcı adı/şifre ile giriş
- ✅ Tüm dersleri ve kontenjanları görüntüleme
- ✅ Derse yapılan başvuruları listeleme
- ✅ Başvuruları onaylama/reddetme
- ✅ Kontenjan kontrolü

---

## 🛠️ Teknoloji Stack

### Backend
- **Framework**: .NET 8 (ASP.NET Core Web API)
- **ORM**: Entity Framework Core 8.0.11
- **Database**: Microsoft SQL Server 2022
- **Logging**: Serilog (Console + File)
- **Validation**: FluentValidation
- **Mapping**: AutoMapper

### Frontend
- **Framework**: ASP.NET Core MVC (Razor Views)
- **CSS Framework**: Bootstrap 5
- **Session Management**: ASP.NET Core Session

### DevOps & Tools
- **Containerization**: Docker + Docker Compose
- **Testing**: xUnit, Moq, FluentAssertions
- **Version Control**: Git

---

## 🏗️ Proje Mimarisi

Proje **Clean Architecture** prensiplerine göre katmanlı bir mimari ile geliştirilmiştir:

```
YazOkulu/
│
├── src/
│   ├── YazOkulu.API/              # REST API
│   ├── YazOkulu.Web/              # MVC Razor UI
│   ├── YazOkulu.Application/      # Business Logic, Services, DTOs, Validators
│   ├── YazOkulu.Domain/           # Entities, Enums, Domain Interfaces
│   └── YazOkulu.Infrastructure/   # EF Core, Repositories, External Services
│
├── tests/
│   └── YazOkulu.Tests/            # Unit & Integration Tests
│
├── docs/
│   └── yaz_okulu_basvuru_modulu.md
│
├── docker-compose.yml
├── Dockerfile.api
├── Dockerfile.web
└── README.md
```

### Katman Bağımlılıkları

```
┌─────────────────────────────────────┐
│   Presentation (API + MVC)          │  ← User Interface
└────────────┬────────────────────────┘
             │ depends on
             ↓
┌─────────────────────────────────────┐
│   Application (Services, DTOs)      │  ← Business Logic
└────────────┬────────────────────────┘
             │ depends on
             ↓
┌─────────────────────────────────────┐
│   Domain (Entities, Interfaces)     │  ← Core Business Rules
└─────────────────────────────────────┘
             ↑
             │ implements
┌────────────┴────────────────────────┐
│   Infrastructure (EF Core, Repos)   │  ← External Concerns
└─────────────────────────────────────┘
```

---

## 📦 Kurulum

### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2022](https://www.microsoft.com/sql-server) veya Docker
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (opsiyonel)

### 1. Repository'yi Klonlayın

```bash
git clone <repository-url>
cd Kontenjan
```

### 2. Projeyi Çalıştırın

#### Seçenek A: Docker ile Çalıştırma (Önerilen - En Kolay)

```bash
# Tek komut! Veritabanı otomatik oluşur, migration otomatik çalışır
docker-compose up -d

# Logları izle
docker-compose logs -f
```

**Not:** Docker ile çalıştırdığınızda:
- ✅ SQL Server otomatik başlar
- ✅ Veritabanı otomatik oluşturulur (migration)
- ✅ Örnek veriler otomatik eklenir (seeding)
- ✅ API ve Web servisleri otomatik başlar

**Manuel adım gerekmez!**

#### Seçenek B: Manuel Çalıştırma (Geliştiriciler için)

**2.1. Connection String Ayarları**

`src/YazOkulu.API/appsettings.json` dosyasında bağlantıyı kontrol edin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=YazOkuluDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

**2.2. Database Migration (Sadece ilk kurulumda)**

```bash
cd src/YazOkulu.API
dotnet ef database update
```

**2.3. Servisleri Başlat**

```bash
# Terminal 1 - API
cd src/YazOkulu.API
dotnet run

# Terminal 2 - Web
cd src/YazOkulu.Web
dotnet run
```

### 5. Tarayıcıdan Erişim

- **Web UI**: http://localhost:5001
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

---

## 💻 Kullanım

### Öğrenci Girişi

1. Web arayüzüne gidin: http://localhost:5001
2. "Öğrenci Girişi" butonuna tıklayın
3. GSM numaranızı girin (örnek: 05001234567)
4. "Kod Gönder" butonuna tıklayın
5. **API log dosyasından** veya **console çıktısından** 6 haneli OTP kodunu bulun
6. Bulunan kodu giriş ekranına yazın
7. Derslere göz atın ve başvurun!

**⚠️ ÖNEMLİ - SMS Sistemi Hakkında:**
- Bu projede **gerçek SMS gönderimi yapılmamaktadır**
- OTP kodları **sadece log dosyasına yazılır**
- Telefonunuza SMS **gelmeyecektir**
- Kodu görmek için `logs/log-YYYYMMDD.txt` dosyasını kontrol edin

### Admin Girişi

1. Web arayüzünde "Admin Paneli" butonuna tıklayın
2. Varsayılan bilgilerle giriş yapın:
   - **Kullanıcı Adı**: `admin`
   - **Şifre**: `Admin123!`
3. Başvuruları görüntüleyin ve onaylayın/reddedin

---

## 📚 API Dokümantasyonu

### Authentication Endpoints

#### Request OTP
```http
POST /api/auth/request-otp
Content-Type: application/json

{
  "phoneNumber": "05001234567"
}
```

#### Verify OTP
```http
POST /api/auth/verify-otp
Content-Type: application/json

{
  "phoneNumber": "05001234567",
  "code": "123456"
}
```

### Course Endpoints

#### Get All Courses
```http
GET /api/courses?studentId={studentId}
```

#### Get Course By ID
```http
GET /api/courses/{courseId}?studentId={studentId}
```

### Application Endpoints

#### Create Application
```http
POST /api/course-applications?studentId={studentId}
Content-Type: application/json

{
  "courseId": 1
}
```

#### Get Student Applications
```http
GET /api/me/applications?studentId={studentId}
```

### Admin Endpoints

#### Get Course Applications (Admin)
```http
GET /api/courses/{courseId}/applications
```

#### Update Application Status (Admin)
```http
PUT /api/course-applications/{id}/status
Content-Type: application/json

{
  "status": 1,
  "reviewNotes": "Onaylandı"
}
```

**Status Values:**
- `0` = Pending (Beklemede)
- `1` = Approved (Onaylandı)
- `2` = Rejected (Reddedildi)

### Swagger Dokümantasyonu

API'nin tüm endpoint'lerini test etmek için Swagger UI'ı kullanın:

http://localhost:5000/swagger

---

## 🐳 Docker

### Docker Compose ile Çalıştırma

```bash
# Tüm servisleri başlat (İLK KULLANIM - Her şey otomatik)
docker-compose up -d

# Logları izle
docker-compose logs -f

# Servisleri durdur
docker-compose down

# Servisleri durdur ve volume'leri temizle (veritabanını da siler)
docker-compose down -v

# Yeniden build et (klasör yapısı değişirse)
docker-compose build --no-cache
docker-compose up -d
```

### Otomatik Özellikler ✨

Docker ile çalıştırdığınızda **HIÇBIR MANUEL İŞLEM GEREKMEz**:

1. ✅ SQL Server container otomatik başlar
2. ✅ API başlarken veritabanını otomatik oluşturur (EF Core Migration)
3. ✅ Örnek dersleri otomatik ekler (Database Seeding)
4. ✅ Web UI otomatik başlar
5. ✅ Health check'ler container'ların hazır olmasını garanti eder

**Hoca için tek komut yeterli:**
```bash
docker-compose up -d
```

### Docker Compose Servisleri

| Servis | Açıklama | Depends On |
|--------|----------|------------|
| **sqlserver** | MS SQL Server 2022 | - |
| **api** | Yaz Okulu Web API | sqlserver (healthy) |
| **web** | Yaz Okulu Web UI | api |

### Portlar

| Servis | Port | URL |
|--------|------|-----|
| Web UI | 5001 | http://localhost:5001 |
| API | 5000 | http://localhost:5000 |
| Swagger | 5000 | http://localhost:5000/swagger |
| SQL Server | 1433 | localhost:1433 |

---

## 🧪 Testing

```bash
# Tüm testleri çalıştır
dotnet test

# Coverage report ile
dotnet test --collect:"XPlat Code Coverage"
```

---

## 📊 Database Şeması

### Tablolar

#### Students
- Id (PK)
- PhoneNumber (Unique)
- FirstName
- LastName
- Email
- StudentNumber
- Department
- CreatedAt, UpdatedAt, IsDeleted

#### Courses
- Id (PK)
- Code (Unique)
- Name
- Quota
- Department
- Faculty
- Instructor
- Credits
- CreatedAt, UpdatedAt, IsDeleted

#### CourseApplications
- Id (PK)
- StudentId (FK)
- CourseId (FK)
- Status (Enum: Pending, Approved, Rejected)
- AppliedAt
- ReviewedAt
- ReviewNotes
- CreatedAt, UpdatedAt, IsDeleted
- **Unique Constraint**: (StudentId, CourseId)

#### OtpCodes
- Id (PK)
- PhoneNumber
- Code (6 digits)
- ExpiresAt
- Status (Enum: Active, Used, Expired)
- UsedAt
- CreatedAt, UpdatedAt, IsDeleted

---

## 🔒 Güvenlik

- ✅ Input validation (FluentValidation)
- ✅ SQL Injection koruması (EF Core Parameterized Queries)
- ✅ XSS koruması (Razor automatic encoding)
- ✅ HTTPS zorunluluğu
- ✅ CORS yapılandırması
- ✅ Soft delete (veri güvenliği)
- ⚠️ **Not**: SMS doğrulama şu anda mock edilmiştir. Production'da gerçek bir SMS provider entegre edilmelidir.

---

## 📝 Seed Data

Sistemde varsayılan olarak aşağıdaki dersler tanımlıdır:

| Kod | Ders Adı | Kontenjan | Fakülte |
|-----|----------|-----------|---------|
| CS101 | Bilgisayar Bilimine Giriş | 30 | Mühendislik Fakültesi |
| CS201 | Veri Yapıları ve Algoritmalar | 25 | Mühendislik Fakültesi |
| CS301 | Veritabanı Yönetim Sistemleri | 20 | Mühendislik Fakültesi |
| MAT101 | Matematik I | 40 | Fen Fakültesi |
| FIZ101 | Fizik I | 35 | Fen Fakültesi |
| ING201 | İleri İngilizce | 15 | Edebiyat Fakültesi |

---

## 🎯 İş Kuralları

1. Öğrenci bir derse **sadece 1 kez** başvurabilir
2. Kontenjanı **dolu** derslere başvuru yapılamaz
3. SMS OTP kodu **5 dakika** geçerlidir
4. Soft delete kullanılır, veriler fiziksel olarak silinmez
5. Başvuru durumları:
   - **Pending**: Beklemede
   - **Approved**: Onaylandı (kontenjan azalır)
   - **Rejected**: Reddedildi

---

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'feat: Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

---

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

---

## 👥 İletişim

**Proje Sahibi**: Alacam Bilişim Development Team

**Proje Linki**: [GitHub Repository](https://github.com/yourusername/yazokulu)

---

## 🙏 Teşekkürler

Bu proje Haliç Üniversitesi Yaz Okulu için geliştirilmiştir.

---

**Geliştirme Tarihi**: Kasım 2025

**Versiyon**: 1.0.0

**Teslim Tarihi**: 21.11.2025
