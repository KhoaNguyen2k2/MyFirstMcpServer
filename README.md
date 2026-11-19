# MyFirstMcpServer

MCP Server đầu tiên viết bằng C# (.NET 8), kết nối SQL Server qua Entity Framework Core và cung cấp các công cụ (MCP tools) để tìm kiếm, lọc và xem thống kê về catalog sách trong thư viện.

## Cấu trúc dự án

```
MyFirstMcpServer/
├── MyFirstMcpServer.sln              # Visual Studio solution
├── MyFirstMcpServer/
│   ├── Program.cs                    # Entry point: đăng ký MCP server, migrate & seed DB
│   ├── MyFirstMcpServer.csproj       # Cấu hình project & dependencies
│   ├── appsettings.json              # Connection string SQL Server
│   ├── Data/
│   │   ├── Book.cs                   # Entity Book
│   │   ├── LibraryDbContext.cs       # DbContext (EF Core)
│   │   └── LibrarySeeder.cs          # Seed 5.000 sách ngẫu nhiên bằng Bogus
│   ├── Tools/
│   │   └── LibraryDbTools.cs         # MCP tools (SearchBooks, GetBooksByGenre, GetLibraryStats)
│   └── Migrations/                   # Migration EF Core (InitialCreate, AddBookColumns)
└── .claude/
    └── settings.local.json
```

## Công nghệ sử dụng

| Thành phần | Phiên bản | Mục đích |
|------------|-----------|----------|
| .NET SDK | 8.0 | Runtime & build |
| `ModelContextProtocol` | 2.4.0 | Framework MCP Server |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.11 | ORM kết nối SQL Server |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.11 | CLI migration |
| `Bogus` | 35.6.5 | Sinh dữ liệu giả cho seed |
| `Microsoft.Extensions.Hosting` | 8.0.1 | Generic host |

## Cách cài đặt và chạy

### 1. Yêu cầu

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB) hoặc SQL Server Express
- Một MCP client (ví dụ: Claude Code, VS Code Copilot, v.v.)

### 2. Build & chạy

```powershell
# Vào thư mục project
cd MyFirstMcpServer

# Restore dependencies
dotnet restore

# Build
dotnet build

# Chạy server
dotnet run
```

Khi chạy đầu tiên, ứng dụng sẽ:
1. Tự động áp dụng migration (`db.Database.Migrate()`)
2. Seed 5.000 bản ghi sách vào database nếu bảng trống (`LibrarySeeder.SeedAsync`)

Server giao tiếp qua **stdio transport** (stdin/stdout), dùng cho MCP client.

## Cấu hình (`appsettings.json`)

Connection string mặc định dùng SQL Server LocalDB:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyFirstMcpDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Nếu bạn dùng SQL Server khác, hãy chỉnh chuỗi `DefaultConnection` trong `MyFirstMcpServer/appsettings.json`.

## Các MCP tools có sẵn

Tất cả tools được khai báo trong `Tools/LibraryDbTools.cs` với attribute `[McpServerTool]`.

### 1. `SearchBooks`

Tìm sách theo tiêu đề hoặc tác giả, trả về tối đa 10 kết quả khớp nhất.

- Tham số: `searchTerm` (chuỗi tìm kiếm)
- Ví dụ: `SearchBooks("harry potter")` hoặc `SearchBooks("tolkien")`

### 2. `GetBooksByGenre`

Liệt kê tối đa 10 sách thuộc một thể loại cụ thể.

- Tham số: `genre` (tên thể loại)
- Ví dụ thể loại có sẵn: `Fantasy`, `Science Fiction`, `Mystery`, `Romance`, `History`, `Biography`, `Thriller`, `Horror`, `Poetry`, `Self-Help`

### 3. `GetLibraryStats`

Trả về thống kê tổng quan của catalog sách: tổng số sách, tổng số bản sao có sẵn, phân bố theo thể loại.

- Không cần tham số

## Migration & Seed

Dự án dùng Entity Framework Core migrations. Các migration đã có trong thư mục `Migrations/`:

- `20261108034309_InitialCreate` — tạo bảng `Books`
- `20261108043453_AddBookColumns` — bổ sung các cột cho bảng `Books`

Để tạo migration mới sau khi thay đổi model:

```powershell
dotnet ef migrations add TenMigrationMoi
dotnet ef database update
```

Dữ liệu mẫu được sinh tự động khi khởi động qua `LibrarySeeder` (dùng `Bogus` với seed cố định `2026` để kết quả khả tái lập).

## Kết nối với MCP client

Vì server dùng stdio transport, bạn có thể thêm nó vào MCP client (ví dụ Claude Code) bằng cách cấu hình lệnh chạy:

```json
{
  "mcpServers": {
    "my-first-mcp-server": {
      "command": "dotnet",
      "args": ["run"],
      "cwd": "I:\\hoctap\\C#\\MyFirstMcpServer\\MyFirstMcpServer"
    }
  }
}
```

## Tác giả & giấy phép

Dự án học tập, tạo ra để làm quen với Model Context Protocol trong ecosystem .NET.

---

*Tạo file README này dựa trên mã nguồn của dự án `MyFirstMcpServer`.*