# LabelForge Studio

A cross-platform label design, barcode design, data-driven printing, print automation, and centralized print management software.

## Architecture

LabelForge Studio consists of four major components:

```
LabelForge Studio
├── Desktop Designer App (.NET MAUI)
├── Local Print Agent (.NET Worker Service)
├── Automation Server (ASP.NET Core Web API)
└── Central System Database (PostgreSQL/SQL Server/SQLite)
```

## Solution Structure

```
LabelForge.sln
├── src/LabelForge.Designer/              # .NET MAUI Desktop Designer App
├── src/LabelForge.Core/                  # Shared models, interfaces, enums
├── src/LabelForge.Database/              # EF Core DbContext, entities, migrations
├── src/LabelForge.Server/                # ASP.NET Core Web API (Automation Server)
├── src/LabelForge.PrintAgent/            # .NET Worker Service (Print Agent)
└── tests/
    ├── LabelForge.Core.Tests/
    ├── LabelForge.Server.Tests/
    └── LabelForge.PrintAgent.Tests/
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- .NET MAUI workload (`dotnet workload install maui`)
- PostgreSQL (for server/database) or SQLite (for local mode)

### Build

```bash
# Build the entire solution (all projects except iOS targets)
dotnet build LabelForge.sln

# Build individual projects
dotnet build src/LabelForge.Core/LabelForge.Core.csproj
dotnet build src/LabelForge.Database/LabelForge.Database.csproj
dotnet build src/LabelForge.Server/LabelForge.Server.csproj
dotnet build src/LabelForge.PrintAgent/LabelForge.PrintAgent.csproj

# Build the MAUI Desktop Designer (macOS)
dotnet build src/LabelForge.Designer/LabelForge.Designer.csproj -f net10.0-maccatalyst
```

### Run

```bash
# Run the MAUI Desktop Designer on macOS
dotnet run --project src/LabelForge.Designer/LabelForge.Designer.csproj -f net10.0-maccatalyst

# Run the Automation Server
dotnet run --project src/LabelForge.Server/LabelForge.Server.csproj

# Run the Print Agent
dotnet run --project src/LabelForge.PrintAgent/LabelForge.PrintAgent.csproj
```

The Automation Server requires a database. Configure it in `src/LabelForge.Server/appsettings.json`:

- **PostgreSQL** (default): Set `"DatabaseProvider": "PostgreSQL"` and update the `"DefaultConnection"` string
- **SQLite** (local dev): Set `"DatabaseProvider": "SQLite"` and change `"DefaultConnection"` to `"Data Source=labelforge.db"`

### Run Tests

```bash
dotnet test tests/LabelForge.Core.Tests/LabelForge.Core.Tests.csproj
```

## Components

### Desktop Designer App (LabelMaker)

The MAUI desktop application for designing labels, barcodes, and RFID tags:

- WYSIWYG canvas designer with drag & drop
- Text, barcode, QR code, image, shape, and line elements
- Object properties panel with full property editing
- Rulers, grid overlay, and snap-to-grid
- Undo/redo system
- Template save/load (JSON format)
- Print dialog with printer selection
- ZPL, EPL, and CPCL label printer language output

### LabelForge.Core

Shared library with domain models, interfaces, and enums:

- **Models**: LabelTemplate, TemplateVersion, LabelElement hierarchy, PrintJob, PrinterInfo, User, Role, Permission, DataSource, Integration, AuditLog, GlobalVariable
- **Interfaces**: ITemplateService, IPrinterService, IPrintService, IPrintJobService, IDataSourceService, IUserService, IAuditService, IIntegrationService
- **Enums**: TemplateStatus, ElementType, BarcodeType, PrintJobStatus, PrinterStatus, DataSourceType, UserRole, etc.

### Automation Server (LabelForge.Server)

ASP.NET Core Web API for print automation:

- **REST API**: Template CRUD, print job creation/status/cancel/retry, printer discovery/status
- **Authentication**: JWT token-based authentication
- **Background Services**: Print job processor, printer status updater, file drop watcher
- **Database**: EF Core with PostgreSQL/SQL Server/SQLite support
- **Integrations**: Webhook, file drop, email, database trigger, scheduled jobs

### Local Print Agent (LabelForge.PrintAgent)

.NET Worker Service for printer communication:

- Printer discovery (Windows PowerShell, macOS lpstat)
- Print queue management
- Silent printing support
- Platform-specific printer services

### LabelForge.Database

EF Core database context and entity configuration:

- Full schema matching the requirement document
- PostgreSQL, SQL Server, and SQLite provider support
- Database seeding for default roles, permissions, and settings
- Migration support

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/login | User login |
| POST | /api/auth/refresh-token | Refresh JWT token |
| GET | /api/templates | List templates |
| GET | /api/templates/{id} | Get template |
| POST | /api/templates | Create template |
| PUT | /api/templates/{id} | Update template |
| DELETE | /api/templates/{id} | Delete template |
| POST | /api/templates/{id}/preview | Preview template |
| POST | /api/templates/{id}/submit | Submit for approval |
| POST | /api/templates/{id}/approve | Approve template |
| POST | /api/templates/{id}/reject | Reject template |
| GET | /api/printers | List printers |
| GET | /api/printers/{id}/status | Get printer status |
| POST | /api/print/jobs | Create print job |
| GET | /api/print/jobs/{id} | Get job status |
| POST | /api/print/jobs/{id}/cancel | Cancel print job |
| POST | /api/print/jobs/{id}/retry | Retry print job |
| GET | /api/audit/logs | Get audit logs |
| GET | /api/integrations | List integrations |
| POST | /api/integrations | Create integration |
| GET | /api/global-variables | List global variables |
| POST | /api/global-variables | Create global variable |

## Technology Stack

| Component | Technology |
|-----------|-----------|
| Desktop App | .NET MAUI 9.0/10.0, C#, XAML, SkiaSharp |
| Core Library | .NET 9.0/10.0, C# |
| Automation Server | ASP.NET Core 10.0 Web API |
| Print Agent | .NET 10.0 Worker Service |
| Database | PostgreSQL (recommended), SQL Server, SQLite |
| ORM | Entity Framework Core 10.0 |
| Barcode Generation | ZXing.Net |
| Rendering | SkiaSharp |
| Job Scheduling | Quartz.NET |
| Authentication | JWT Bearer |

## Development Phases

| Phase | Focus |
|-------|-------|
| Phase 1 | Solution structure, core models, database schema, basic dashboard |
| Phase 2 | Canvas designer, text/barcode/image/shape objects, properties panel |
| Phase 3 | Data sources (static, CSV, Excel, SQL), field mapping, formulas |
| Phase 4 | Print screen, print preview, PDF export, local print agent |
| Phase 5 | Automation server, REST API, webhook/file drop/email triggers |
| Phase 6 | Users, roles, permissions, revision workflow, audit logs |
| Phase 7 | ZPL/EPL/TSPL output, RFID adapter, batch printing, diagnostics |

## License

Proprietary - All rights reserved.