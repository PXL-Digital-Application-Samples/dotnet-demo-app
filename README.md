# 🧩 .NET User Management API

A lightweight ASP.NET Core Web API for managing users (Create, Read, Update, Delete) with in-memory data storage. Includes auto-seeded user data and interactive Swagger (OpenAPI) documentation served at the root (`/`).

---

## 📦 Features

- ✅ **Clean Architecture**: Proper separation of concerns with Domain, Infrastructure, and Presentation layers
- ✅ In-memory storage for rapid development and testing  
- 🔄 Full CRUD endpoints: `POST`, `GET`, `PUT`, `DELETE`  
- 🧪 Swagger UI for testing & docs at `/` (enabled in all environments)
- 🔧 Configurable port (default: 5000, environment variable, or command line)
- 🚀 Minimal setup, ready for DevOps CI/CD pipelines  
- 🧰 Easily extendable to use persistent databases (e.g., Entity Framework with SQL Server, PostgreSQL)
- 🐧 Cross-platform (runs on Linux, Windows, macOS)
- 🔒 Smart HTTPS redirection (only in Development environment)
- ✅ **Comprehensive Testing**: Unit tests and integration tests with xUnit, Moq, and FluentAssertions

---

## Getting Started

### Requirements

- .NET 8.0 SDK or later

### Installation & Running

```bash
# Clone the repository
# Restore dependencies
dotnet restore

# Run the application (default port 5000)
dotnet run --project src/UserManagementApi.Web

# Or run in development mode with hot reload
dotnet watch run --project src/UserManagementApi.Web
```

### Port Configuration

The API supports flexible port configuration:

**Default (port 5000):**

```bash
dotnet run --project src/UserManagementApi.Web
```

**Using environment variable:**

```bash
# Windows PowerShell
$env:PORT="5001"; dotnet run --project src/UserManagementApi.Web

# Linux/macOS
PORT=5001 dotnet run --project src/UserManagementApi.Web
```

**Using command line:**

```bash
dotnet run --project src/UserManagementApi.Web --urls "http://localhost:5002"
```

The API will be available at:

- **Default**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000` (root path, available in all environments)

### Project Structure

```text
UserManagementApi/
├── src/
│   ├── UserManagementApi.Core/           # Domain Layer
│   │   ├── Entities/                     # Domain entities
│   │   │   └── User.cs
│   │   ├── Interfaces/                   # Repository & service interfaces
│   │   │   ├── IUserRepository.cs
│   │   │   └── IUserService.cs
│   │   └── Services/                     # Business logic services
│   │       └── UserService.cs
│   ├── UserManagementApi.Infrastructure/ # Data Access Layer
│   │   └── Repositories/                 # Data access implementations
│   │       └── InMemoryUserRepository.cs
│   └── UserManagementApi.Web/            # Presentation Layer
│       ├── Controllers/                  # API controllers
│       │   └── UsersController.cs
│       ├── Models/                       # Request/response models
│       │   └── UserModels.cs
│       ├── Program.cs                    # Application entry point
│       └── appsettings.json
├── tests/
│   ├── UserManagementApi.UnitTests/      # Unit tests
│   │   ├── Core/Services/
│   │   │   └── UserServiceTests.cs
│   │   └── Infrastructure/Repositories/
│   │       └── InMemoryUserRepositoryTests.cs
│   └── UserManagementApi.IntegrationTests/ # Integration tests
│       └── UsersControllerIntegrationTests.cs
├── UserManagementApi.sln                 # Solution file
└── README.md
```

### Testing

```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test tests/UserManagementApi.UnitTests

# Run only integration tests  
dotnet test tests/UserManagementApi.IntegrationTests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests with detailed output
dotnet test --verbosity normal
```

**Test Coverage:**

- **Unit Tests**: 41 tests covering business logic and repository operations
- **Integration Tests**: End-to-end API testing with real HTTP requests
- **Test Stack**: xUnit, Moq (mocking), FluentAssertions (assertions), ASP.NET Core Test Host

### API Endpoints

**Base URL:** `http://localhost:5000/api`

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

**Example Usage:**

```bash
# Get all users
curl http://localhost:5000/api/users

# Get specific user
curl http://localhost:5000/api/users/1

# Create new user
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"John Smith","email":"john.smith@example.com"}'

# Update user
curl -X PUT http://localhost:5000/api/users/1 \
  -H "Content-Type: application/json" \
  -d '{"name":"John Updated","email":"john.updated@example.com"}'

# Delete user
curl -X DELETE http://localhost:5000/api/users/1
```

### Environment Variables

- `PORT` - Port number (default: 5000)
- `ASPNETCORE_ENVIRONMENT` - Environment (Development, Production)

### Production Deployment

For production deployment, consider:

- Setting `ASPNETCORE_ENVIRONMENT=Production`
- Configuring proper HTTPS certificates
- Replacing in-memory storage with a persistent database
- Adding authentication and authorization
- Implementing proper logging and monitoring
