# Lafarge Onboarding Process API

A comprehensive .NET 8 API for managing the Lafarge employee onboarding process, featuring user authentication, document uploads, content management, image gallery, and role-based access control with advanced performance optimizations.

## Features

- **User Authentication & Authorization**: JWT-based authentication with role-based access control
- **Document Management**: Upload and manage onboarding documents (PDFs, images, etc.)
- **Content Management**: Upload and manage Word documents for local hire info, welcome messages, onboarding plans, and etiquette
- **Image Gallery**: Upload, manage, and serve compressed images (CEO, HR, General) with Base64 encoding
- **Contact Management**: Manage local and global contact information with CSV import/export
- **App Version Control**: Manage mobile app versions with update notifications
- **User Management**: Create, view, and manage users with different roles and bulk upload support
- **Performance Optimizations**: Response compression, ETag caching, memory caching, and optimized queries
- **PostgreSQL Database**: Robust data persistence with Entity Framework Core
- **Swagger UI**: Interactive API documentation with JWT authentication support
- **Docker Support**: Containerized deployment with Docker Compose and Cloud Run compatibility

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
- `POST /api/users/bulk-upload` - Upload multiple users via CSV

### Documents
- `POST /api/documentsupload/upload/*` - Upload specific document types (HR Admin only)
- `GET /api/documentsupload/all` - View all documents (paginated)
- `GET /api/documentsupload/{id}` - Get document by ID

### Content Management (HR Admin upload, All users view)
- `POST /api/content/local-hire-info-upload` - Upload local hire info document
- `GET /api/content/get-local-hire-info` - Get local hire information
- `POST /api/content/upload-welcome-messages` - Upload welcome message documents
- `GET /api/content/get-welcome-messages` - Get welcome messages
- `POST /api/content/upload-onboarding-plan` - Upload onboarding plan document
- `GET /api/content/get-onboarding-plan` - Get onboarding plan
- `POST /api/content/upload-etiquette` - Upload etiquette document
- `GET /api/content/get-etiquette` - Get etiquette information
- `DELETE /api/content/delete-*` - Delete latest content (HR Admin only)

### Image Gallery (HR Admin upload, All users view)
- `POST /api/gallery/upload-ceo-image` - Upload CEO image
- `POST /api/gallery/upload-hr-image` - Upload HR image
- `POST /api/gallery/upload-any-image` - Upload general image
- `GET /api/gallery/get-ceo-images` - Get CEO images
- `GET /api/gallery/get-hr-images` - Get HR images
- `GET /api/gallery/get-any-images` - Get general images
- `DELETE /api/gallery/delete-*` - Delete images (HR Admin only)

### Contacts
- `GET /api/contacts/local-contacts` - Get local contacts
- `POST /api/contacts/upload-local-contacts` - Upload local contacts CSV (HR Admin only)
- `GET /api/contacts/all-contacts` - Get all contacts by category
- `POST /api/contacts/upload-all-contacts` - Upload all contacts CSV (HR Admin only)

### App Version Control
- `POST /api/appversioncheck/create` - Create new app version (HR Admin only)
- `GET /api/appversioncheck/latest` - Get latest app version by name

## Environment Variables

The application uses the following environment variables (configured in docker-compose.yml):

### Database Configuration
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `DB_CONNECTION_NAME` - Cloud SQL connection name (for Google Cloud deployment)
- `DB_NAME` - Database name
- `DB_USER` - Database username
- `POSTGRES_PWD` - Database password

### JWT Configuration
- `Jwt__Key` - JWT signing key
- `Jwt__Issuer` - JWT issuer
- `Jwt__Audience` - JWT audience

### Application Configuration
- `PORT` - Application port (defaults to 8080)
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ASPNETCORE_URLS` - Application URLs

## Database Schema

The application uses PostgreSQL with Entity Framework Core migrations. Key tables include:

### User Management
- **AspNetUsers** - User accounts with ASP.NET Identity
- **AspNetRoles** - User roles (LOCAL_HIRE, EXPAT, VISITOR, HR_ADMIN)
- **AspNetUserRoles** - User-role relationships

### Content Management
- **OnboardingDocuments** - Uploaded documents with metadata
- **LocalHireInfos** - Structured local hire information
- **WelcomeMessages** - CEO and HR welcome messages
- **OnboardingPlans** - Onboarding timeline and buddy information
- **Etiquettes** - Cultural and regional etiquette information

### Media & Contacts
- **Galleries** - Image storage with Base64 encoding and compression
- **Contacts** - Local contact information
- **AllContacts** - Global contact information by category
- **AppVersions** - Mobile app version management

### Seeded Roles
- `LOCAL_HIRE` - Local employees
- `EXPAT` - Expatriate employees
- `VISITOR` - Temporary visitors
- `HR_ADMIN` - HR administrators with full access

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

# Database operations
docker compose exec postgres psql -U postgres -d lafarge_onboarding_db
```

## Development Commands

```bash
# Entity Framework migrations
cd Lafarge-Onboarding.infrastructure
dotnet ef migrations add MigrationName --startup-project ../Lafarge-Onboarding.api
dotnet ef database update --startup-project ../Lafarge-Onboarding.api

# Run application locally
cd Lafarge-Onboarding.api
dotnet run

# Build and test
dotnet build
dotnet test
```

## Project Structure

```
├── Lafarge-Onboarding.api/          # API layer
│   ├── Controllers/                 # API controllers
│   │   ├── AuthController.cs        # Authentication endpoints
│   │   ├── UsersController.cs       # User management
│   │   ├── DocumentsUploadController.cs # Document uploads
│   │   ├── ContentController.cs     # Content management
│   │   ├── GalleryController.cs     # Image gallery
│   │   ├── ContactsController.cs    # Contact management
│   │   └── AppVersionCheckController.cs # App version control
│   ├── ConfigExtension/             # Configuration extensions
│   ├── Middleware/                  # Custom middleware
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   └── ETagMiddleware.cs        # HTTP caching
│   ├── Formats/                     # CSV format templates
│   ├── Program.cs                   # Application entry point
│   ├── GlobalUsings.cs              # Global using statements
│   └── appsettings*.json            # Configuration files
├── Lafarge-Onboarding.application/  # Application services
│   ├── Services/                    # Business logic services
│   │   ├── CachedLocalHireInfoService.cs # Cached content services
│   │   ├── CachedWelcomeMessageService.cs
│   │   ├── GalleryService.cs        # Image processing & compression
│   │   └── [Other services...]      # Auth, Users, Documents, etc.
│   ├── Abstraction/                 # Service interfaces
│   ├── ServiceRegistrations/        # DI configuration
│   └── GlobalUsings.cs              # Global using statements
├── Lafarge-Onboarding.domain/       # Domain entities
│   ├── Entities/                    # Domain models
│   │   ├── Users.cs, Role.cs        # Identity entities
│   │   ├── Gallery.cs               # Image gallery
│   │   ├── LocalHireInfo.cs         # Content entities
│   │   └── [Other entities...]      # Documents, Contacts, etc.
│   ├── Dtos/                        # Data transfer objects
│   └── GlobalUsings.cs              # Global using statements
└── Lafarge-Onboarding.infrastructure/ # Infrastructure layer
    ├── Data/                        # Database context
    ├── Repositories/                # Data access layer
    ├── Migrations/                  # EF Core migrations
    ├── ServiceRegistrations/        # Infrastructure DI
    └── GlobalUsings.cs              # Global using statements
```

## Performance & Security Features

### Performance Optimizations
- **Response Compression**: Gzip compression reducing bandwidth by 60-80%
- **ETag Caching**: HTTP caching with MD5-based ETags for 304 responses
- **Memory Caching**: Write-through caching for content with indefinite duration
- **Image Compression**: Aggressive JPEG (quality 60) and PNG (Level 9) compression
- **Optimized Queries**: Projection queries and efficient database operations

### Security
- **JWT Bearer Authentication**: Secure token-based authentication
- **Role-based Authorization**: Granular access control (HR_ADMIN, LOCAL_HIRE, EXPAT, VISITOR)
- **Password Security**: ASP.NET Core Identity with strong password requirements
- **Input Validation**: File type validation and size limits
- **HTTPS Support**: SSL/TLS encryption in production
- **User Tracking**: Audit trails with user identification for uploads

### Caching Strategy
- **Write-through Cache**: Cache invalidation on database updates
- **Indefinite Duration**: Cache persists until manual removal
- **High Priority**: Memory cache with high priority for content
- **Decorator Pattern**: Cached service implementations

## Deployment

### Docker Compose (Local Development)
```bash
docker compose up --build
```

### Google Cloud Run (Production)
The application is optimized for Cloud Run deployment with:
- Dynamic port configuration via `PORT` environment variable
- Cloud SQL socket connections
- Efficient resource usage
- Health checks and logging

### Performance Monitoring
- Structured logging with Serilog
- File and console logging
- Request/response tracking
- Error handling middleware

## File Format Support

### Document Uploads
- **Word Documents**: .docx, .doc (for content management)
- **Images**: .jpg, .jpeg, .png, .gif (for gallery)
- **CSV Files**: Contact and user bulk uploads

### Content Processing
- **Text Extraction**: Word document parsing with section detection
- **Image Compression**: Automatic compression with quality optimization
- **Base64 Encoding**: Efficient image storage and retrieval

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.