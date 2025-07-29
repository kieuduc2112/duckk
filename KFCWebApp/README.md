# 🍗 KFC Web Application

Ứng dụng web quản lý đặt hàng KFC được xây dựng bằng ASP.NET Core MVC.

## 📋 Tính năng

### 👥 Người dùng
- Xem danh sách sản phẩm và combo
- Tìm kiếm và lọc sản phẩm theo danh mục
- Thêm sản phẩm vào giỏ hàng
- Đặt hàng và thanh toán
- Xem lịch sử đơn hàng
- Áp dụng mã giảm giá

### 👨‍💼 Admin
- Quản lý sản phẩm (thêm, sửa, xóa)
- Quản lý danh mục sản phẩm
- Quản lý combo và khuyến mãi
- Quản lý đơn hàng
- Quản lý mã giảm giá
- Quản lý người dùng

## 🛠️ Yêu cầu hệ thống

- **.NET 8.0 SDK** hoặc cao hơn
- **SQL Server** hoặc **SQL Server Express**
- **Visual Studio 2022** hoặc **Visual Studio Code**

## 📦 Cài đặt

### 1. Clone repository
```bash
git clone https://github.com/kieuduc2112/duckk.git
cd duckk/KFCWebApp
```

### 2. Cài đặt dependencies
```bash
dotnet restore
```

### 3. Cấu hình database

#### Cách 1: Sử dụng SQL Server LocalDB (Khuyến nghị cho development)
- Cài đặt SQL Server LocalDB
- Chạy lệnh sau để tạo database:
```bash
dotnet ef database update
```

#### Cách 2: Sử dụng SQL Server
- Mở file `appsettings.json`
- Cập nhật connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=KFCWebApp;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 4. Chạy ứng dụng
```bash
dotnet run
```

Ứng dụng sẽ chạy tại: **http://localhost:5115**

## 🔧 Cấu hình

### Tài khoản Admin mặc định
- **Email:** admin@kfc.com
- **Mật khẩu:** Admin123!

### Tài khoản User mặc định
- **Email:** user@kfc.com
- **Mật khẩu:** User123!

## 📁 Cấu trúc dự án

```
KFCWebApp/
├── Controllers/          # Controllers xử lý request
├── Models/              # Models và Entities
├── Views/               # Razor Views
├── Data/                # Database context và migrations
├── Services/            # Business logic services
├── wwwroot/             # Static files (CSS, JS, Images)
└── Migrations/          # Entity Framework migrations
```

## 🚀 Deployment

### Deploy lên Azure
1. Tạo Azure App Service
2. Cấu hình connection string trong Azure
3. Deploy từ GitHub hoặc Visual Studio

### Deploy lên IIS
1. Publish project: `dotnet publish -c Release`
2. Copy files từ `bin/Release/net8.0/publish` lên IIS
3. Cấu hình connection string trong web.config

## 🐛 Troubleshooting

### Lỗi database connection
- Kiểm tra SQL Server có đang chạy không
- Kiểm tra connection string trong `appsettings.json`
- Chạy `dotnet ef database update` để tạo database

### Lỗi build
- Kiểm tra .NET SDK version: `dotnet --version`
- Xóa thư mục `bin` và `obj`, sau đó chạy `dotnet restore`

### Lỗi port đã được sử dụng
- Thay đổi port trong `Properties/launchSettings.json`
- Hoặc chạy: `dotnet run --urls "http://localhost:5000"`

## 📝 Changelog

### Version 1.0.0
- ✅ Xóa nút "Thêm sản phẩm mới" khỏi trang danh sách sản phẩm
- ✅ Thêm trang Edit và Delete cho Product controller
- ✅ Cải thiện giao diện người dùng
- ✅ Thêm tính năng quản lý combo và khuyến mãi

## 🤝 Đóng góp

1. Fork dự án
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit thay đổi (`git commit -m 'Add some AmazingFeature'`)
4. Push lên branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📄 License

Dự án này được phân phối dưới MIT License. Xem file `LICENSE` để biết thêm chi tiết.

## 📞 Liên hệ

- **GitHub:** [kieuduc2112](https://github.com/kieuduc2112)
- **Repository:** https://github.com/kieuduc2112/duckk

---

⭐ Nếu dự án này hữu ích, hãy cho một star nhé!