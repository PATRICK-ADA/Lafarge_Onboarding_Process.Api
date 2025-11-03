# Lafarge Onboarding Process API

A comprehensive .NET 8 API for managing the Lafarge employee onboarding process, featuring user authentication, document uploads, and role-based access control.

## Features

- **User Authentication & Authorization**: JWT-based authentication with role-based access control
- **Document Management**: Upload and manage onboarding documents (PDFs, images, etc.)
- **User Management**: Create, view, and manage users with different roles
- **PostgreSQL Database**: Robust data persistence with Entity Framework Core
- **Swagger UI**: Interactive API documentation with JWT authentication support
- **Docker Support**: Containerized deployment with Docker Compose

## Prerequisites

- Docker and Docker Compose
- .NET 8 SDK (for local development)

## Quick Start with Docker

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd lafarge-onboarding-process
   ```

2. **Start the application**
   ```bash
   docker compose up --build
   ```

3. **Access the application**
   - API: http://localhost:8080
   - Swagger UI: http://localhost:8080/swagger
   - PostgreSQL: localhost:5432

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token

### Users (HR Admin only)
- `GET /api/users/view-users` - View paginated list of users
- `POST /api/users/bulk-upload` - Upload multiple users

### Documents
- `POST /api/documentsupload/upload/*` - Upload specific document types (HR Admin only)
- `GET /api/documentsupload/all` - View all documents (paginated)
- `GET /api/documentsupload/{id}` - Get document by ID

## Environment Variables

The application uses the following environment variables (configured in docker-compose.yml):

- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `Jwt__Key` - JWT signing key
- `Jwt__Issuer` - JWT issuer
- `Jwt__Audience` - JWT audience

## Database

The application uses PostgreSQL with the following roles seeded by default:
- `LOCAL_HIRE`
- `EXPAT`
- `VISITOR`
- `HR_ADMIN`

## Development

### Local Development Setup

1. **Install dependencies**
   ```bash
   dotnet restore
   ```

2. **Update database**
   ```bash
   cd Lafarge-Onboarding.infrastructure
   dotnet ef database update --startup-project ../Lafarge-Onboarding.api
   ```

3. **Run the application**
   ```bash
   cd Lafarge-Onboarding.api
   dotnet run
   ```

### Building and Testing

```bash
# Build
dotnet build

# Run tests
dotnet test

# Publish
dotnet publish -c Release
```

## Docker Commands

```bash
# Build and start services
docker compose up --build

# Start services in background
docker compose up -d

# Stop services
docker compose down

# View logs
docker compose logs -f

# Rebuild specific service
docker compose up --build lafarge-api
```

## Project Structure

```
├── Lafarge-Onboarding.api/          # API layer
│   ├── Controllers/                 # API controllers
│   ├── Program.cs                   # Application entry point
│   └── appsettings*.json            # Configuration files
├── Lafarge-Onboarding.application/  # Application services
│   ├── Services/                    # Business logic
│   └── Abstraction/                 # Interfaces
├── Lafarge-Onboarding.domain/       # Domain entities
│   ├── Entities/                    # Domain models
│   └── OnboardingRequests/          # Request DTOs
└── Lafarge-Onboarding.infrastructure/ # Infrastructure layer
    ├── Data/                        # Database context
    ├── Repositories/                # Data access
    └── Migrations/                  # Database migrations
```

## Security

- JWT Bearer token authentication
- Role-based authorization
- Password hashing with ASP.NET Core Identity
- HTTPS enabled in production

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.