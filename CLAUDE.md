# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

RRHH (Recursos Humanos) is a comprehensive Human Resources Management System built with .NET 8.0 ASP.NET Core MVC. It manages employee data, payroll, attendance, and various HR processes for Chilean companies.

## Essential Commands

### Build and Run
```bash
# Build the solution
dotnet build RRHH.sln

# Build in Release mode
dotnet build RRHH.sln --configuration Release

# Restore NuGet packages
dotnet restore RRHH.sln

# Run the application (Development environment)
dotnet run --project RRHH/RRHH.csproj

# Run with specific profile
dotnet run --project RRHH/RRHH.csproj --launch-profile https
```

### Application URLs
- **HTTPS**: https://localhost:7272
- **HTTP**: http://localhost:5199
- **IIS Express**: http://localhost:39099

### Code Formatting
```bash
# Format code (requires .NET SDK 6.0+)
dotnet format RRHH.sln

# Format and verify no changes needed
dotnet format RRHH.sln --verify-no-changes
```

**Note**: This project currently lacks unit tests and linting configuration.

## Architecture Overview

### Core Structure
- **Controllers/**: MVC controllers handling HTTP requests for different HR modules
- **Models/**: Data models, ViewModels, and business entities
- **Views/**: Razor views for the web interface
- **Servicios/**: Business logic services for each HR module
- **BaseDatos/**: Database access layer (SQL Server + MongoDB)
- **wwwroot/**: Static web assets (CSS, JS, images)

### Key Technologies
- **.NET 8.0** ASP.NET Core MVC
- **SQL Server** (primary database via Microsoft.Data.SqlClient)
- **MongoDB** (NoSQL for specific features via MongoDB.Driver)
- **Azure Blob Storage** (file storage via Azure.Storage.Blobs)
- **Rollbar** (error tracking and logging)
- **Rotativa** (PDF generation)

### Database Architecture
The application uses a dual-database approach:
- **SQL Server**: Primary relational data (employees, contracts, payroll)
- **MongoDB**: Permissions, profiles, and configuration data

### Service Layer Pattern
Each HR module follows a consistent pattern:
- **Controller**: Handles HTTP requests (`Controllers/[Module]Controller.cs`)
- **Service**: Business logic (`Servicios/[Module]/[Module]Service.cs`)
- **Models**: Data structures (`Models/ViewModels/[Module]BaseVM.cs`)

### Key HR Modules
- **Trabajadores**: Employee management
- **Contratos**: Employment contracts
- **Liquidacion**: Payroll processing
- **Asistencias**: Attendance tracking
- **Haberes/Descuentos**: Salary components (earnings/deductions)
- **Faenas**: Work sites/locations
- **CentrosCostos**: Cost centers
- **Papeletas**: Employee requests/forms

### Configuration Management
- Environment-specific settings in `appsettings.json`
- Database connections vary by environment (Development/Test/Production)
- Azure storage and external API credentials managed per environment
- Session timeout set to 1200 minutes (20 hours)

### Frontend Structure
- Uses **Limitless** admin template with Bootstrap
- Custom JavaScript for each module in `wwwroot/Scripts/`
- DataTables for grid functionality
- jQuery-based form interactions

### Authentication & Authorization
- Custom session-based authentication (`AuthService`)
- Role-based permissions stored in MongoDB
- Profile-based access control (`ProfileService`)

### File Processing
- **Rotativa**: PDF generation (located in `RRHH/Rotativa/`)
- **CSV processing**: Import/export for payroll files
- **Azure Blob Storage**: Document and file management

## Development Guidelines

### Working with Database
- SQL Server operations via `IDatabaseManager` and `DatabaseManager`
- MongoDB operations via `IMongoDBContext` and `MongoCollectionManager`
- Connection strings are environment-specific

### Adding New HR Modules
1. Create controller in `Controllers/`
2. Create service interface and implementation in `Servicios/[Module]/`
3. Create ViewModels in `Models/ViewModels/`
4. Create views in `Views/[Module]/`
5. Add JavaScript functionality in `wwwroot/Scripts/`
6. Register service in `Program.cs` DI container

### Environment Configuration
- **Development**: Local development with remote database
- **Test**: Testing environment
- **Beta**: Pre-production environment  
- **Production**: Live production environment

### Error Handling
- Rollbar integration for error tracking
- Environment-specific error reporting
- Custom error pages in `Views/Shared/`

### PDF Generation
- Uses Rotativa for PDF reports
- Executable files located in `RRHH/Rotativa/`
- Configure with `RotativaConfiguration.Setup()`

### Security Considerations
- Database credentials stored in `appsettings.json` (should be moved to secure configuration)
- JWT configuration for API authentication
- CORS enabled for all origins (should be restricted in production)
- Session-based authentication with extended timeout