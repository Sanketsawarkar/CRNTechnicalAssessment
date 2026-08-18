# CRN Technical Assessment - Project Documentation

## 1. Project Overview

CRN Technical Assessment is a Product Management RESTful API developed using ASP.NET Core Web API and .NET 8.

The solution is designed using a layered architecture to provide separation of concerns, maintainability, scalability, and testability.

The application provides functionality for:

- User authentication
- JWT-based authentication
- Refresh token authentication
- Role-based authorization
- Product CRUD operations
- User management
- Request validation
- Centralized exception handling
- Structured logging
- API versioning
- SQL Server database integration
- Entity Framework Core
- Swagger/OpenAPI documentation
- Docker containerization
- Unit and API testing

---

# 2. Technology Stack

| Technology | Purpose |
|------------|---------|
| .NET 8 | Application framework |
| C# | Programming language |
| ASP.NET Core Web API | REST API development |
| Entity Framework Core | ORM / database access |
| SQL Server | Relational database |
| JWT | Access token authentication |
| Refresh Tokens | Token renewal |
| FluentValidation | Request validation |
| Serilog | Structured logging |
| Swagger / OpenAPI | API documentation |
| Docker | Application containerization |
| Docker Compose | Multi-container orchestration |
| xUnit | Automated testing |
| Moq | Mocking dependencies |
| WebApplicationFactory | API integration testing |

---

# 3. Architecture

The project follows a layered architecture.

```text
CRNTechnicalAssessment
│
├── src
│   │
│   ├── CRNTechnicalAssessment.API
│   │   ├── Controllers
│   │   ├── Middleware
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   │
│   ├── CRNTechnicalAssessment.Application
│   │   ├── DTOs
│   │   ├── Interfaces
│   │   ├── Services
│   │   ├── Settings
│   │   └── Validators
│   │
│   ├── CRNTechnicalAssessment.Domain
│   │   └── Entities
│   │
│   └── CRNTechnicalAssessment.Infrastructure
│       ├── Data
│       │   ├── Configurations
│       │   ├── Repositories
│       │   └── ApplicationDbContext.cs
│       ├── Identity
│       └── Migrations
│
└── tests
    │
    ├── CRNTechnicalAssessment.API.Tests
    ├── CRNTechnicalAssessment.Application.Tests
    └── CRNTechnicalAssessment.Infrastructure.Tests


Layer Responsibilities
API Layer

Responsible for:

HTTP requests and responses
Controllers
API versioning
Authentication and authorization configuration
Exception handling middleware
Swagger/OpenAPI configuration
Application Layer

Responsible for application/business logic.

Contains:

DTOs
Service interfaces
Service implementations
Validators
JWT settings

This layer keeps business operations separated from HTTP and database concerns.

Domain Layer

Contains the core domain entities:

Product
Item
User
RefreshToken

The domain layer does not depend on infrastructure implementation details.

Infrastructure Layer

Responsible for:

Entity Framework Core
SQL Server connectivity
DbContext
Entity configurations
Repository implementations
Authentication-related services
Password hashing
JWT token generation
Refresh token persistence
EF Core migrations
4. Database Design

The application uses SQL Server with Entity Framework Core.

The primary database entities are:

Product
Column	Type	Description
Id	int	Primary key
ProductName	nvarchar(255)	Product name
CreatedBy	nvarchar(100)	User who created the product
CreatedOn	datetime2	Creation timestamp
ModifiedBy	nvarchar(100)	User who last modified the product
ModifiedOn	datetime2	Modification timestamp
Item
Column	Type	Description
Id	int	Primary key
ProductId	int	Foreign key to Product
Quantity	int	Product quantity

The relationship is:

Product
   │
   └── Item
        ProductId → Product.Id
User

The Users table stores:

Username
Password hash
Role
Active status
RefreshToken

The RefreshTokens table stores:

Username
Refresh token
Expiration time
Creation time
Revocation status
5. Entity Framework Core

ApplicationDbContext is responsible for database access.

The context contains:

DbSet<Product>
DbSet<Item>
DbSet<RefreshToken>
DbSet<User>

Entity configurations are applied through:

modelBuilder.ApplyConfigurationsFromAssembly(
    typeof(ApplicationDbContext).Assembly);

This keeps database configuration separate from the entity classes.

6. Database Migrations

EF Core migrations are included in:

src/CRNTechnicalAssessment.Infrastructure/Migrations

The migrations create the required database schema.

The migration history includes:

Initial database creation
Refresh token table
User table
User-related changes

To apply migrations manually:

dotnet ef database update `
  --project src\CRNTechnicalAssessment.Infrastructure `
  --startup-project src\CRNTechnicalAssessment.API
7. Authentication

The application uses JWT-based authentication.

Authentication flow:

Client
  │
  │ Username + Password
  ▼
AuthController
  │
  ▼
AuthService
  │
  ├── Validate User
  │
  ├── Verify Password
  │
  ├── Generate JWT Access Token
  │
  └── Generate Refresh Token
  │
  ▼
Client
Login Flow
Client sends username and password.
API retrieves the user.
Password hash is verified.
JWT access token is generated.
Refresh token is generated.
Refresh token is stored in the database.
Authentication response is returned to the client.

Example:

POST /api/v1/Auth/login

Request:

{
  "username": "admin",
  "password": "Admin@123"
}

The actual credentials depend on the users available in the configured database.

8. Refresh Token Flow

When an access token expires, the client can use the refresh token.

Access Token
     │
     │ Expired
     ▼
Refresh Token
     │
     ▼
AuthService
     │
     ├── Validate token
     ├── Check expiration
     ├── Check revocation
     └── Generate new access token
     │
     ▼
New Access Token

Endpoint:

POST /api/v1/Auth/refresh
9. Role-Based Authorization

The API supports role-based authorization.

The main roles are:

Admin
User

Examples:

Admin

Admins can perform administrative operations such as:

Create products
Update products
Delete products
Manage users
User

Authenticated users can access protected read operations according to the authorization rules configured in the API.

Authorization is implemented using ASP.NET Core authorization policies/roles.

10. API Endpoints
Authentication
Method	Endpoint	Description	Authorization
POST	/api/v1/Auth/login	Authenticate user and generate tokens	Public
POST	/api/v1/Auth/refresh	Generate a new access token	Public
Products
Method	Endpoint	Description	Authorization
GET	/api/v1/Products	Get all products	Authenticated
GET	/api/v1/Products/{id}	Get product by ID	Authenticated
POST	/api/v1/Products	Create product	Admin
PUT	/api/v1/Products/{id}	Update product	Admin
DELETE	/api/v1/Products/{id}	Delete product	Admin
Users
Method	Endpoint	Description	Authorization
GET	/api/v1/Users	Get users	Admin
POST	/api/v1/Users	Create user	Admin
11. Product CRUD Flow

The product request flow is:

HTTP Request
     │
     ▼
ProductsController
     │
     ▼
IProductService
     │
     ▼
ProductService
     │
     ▼
IProductRepository
     │
     ▼
ProductRepository
     │
     ▼
ApplicationDbContext
     │
     ▼
SQL Server

This separation ensures that controllers are not directly responsible for database operations.

12. Repository Pattern

The repository pattern is used to separate data access logic from business logic.

The application contains:

IProductRepository
        │
        ▼
ProductRepository
        │
        ▼
ApplicationDbContext

The service layer communicates with the repository rather than directly handling database operations.

This improves:

Maintainability
Testability
Separation of concerns
13. Request Validation

FluentValidation is used to validate incoming product requests.

Validators are located under:

src/CRNTechnicalAssessment.Application/Validators

Examples include:

CreateProductValidator
UpdateProductValidator

Validation is performed before invalid data reaches the business logic.

Examples of validation scenarios include:

Required product name
Product name length
Required product information
Invalid request data
14. Exception Handling

The application uses centralized exception handling middleware.

Location:

src/CRNTechnicalAssessment.API/Middleware/ExceptionHandlingMiddleware.cs

The middleware provides a centralized location for handling unexpected exceptions and returning consistent API responses.

The flow is:

Controller
    │
    ▼
Service
    │
    ▼
Exception
    │
    ▼
ExceptionHandlingMiddleware
    │
    ▼
Consistent HTTP Error Response

This avoids duplicating exception-handling code across controllers.

15. Logging

Serilog is used for structured application logging.

Logs are stored under:

src/CRNTechnicalAssessment.API/logs

Logging helps with:

Application monitoring
Error investigation
Debugging
Tracking API activity
16. Swagger / OpenAPI

Swagger is configured to provide interactive API documentation.

When running the application locally, Swagger can be accessed through:

http://localhost:8080/swagger/index.html

when the API is running through Docker.

Swagger allows the evaluator to:

View available endpoints
View request/response models
Authenticate using JWT
Execute API requests
Inspect HTTP responses

For protected endpoints, obtain the JWT through the login endpoint and use the Swagger Authorize option.

17. Docker Architecture

The solution is containerized using Docker Compose.

The application contains two main containers:

┌─────────────────────────────┐
│        API Container        │
│                             │
│ ASP.NET Core Web API        │
│ Port: 8080                  │
└──────────────┬──────────────┘
               │
               │ SQL Connection
               ▼
┌─────────────────────────────┐
│      SQL Server Container   │
│                             │
│ SQL Server 2022             │
│ Port: 1433                  │
└─────────────────────────────┘

Docker Compose manages both services.

18. Docker Services

The docker-compose.yml contains:

API
Container:
crntechnicalassessment-api


Port:
8080
SQL Server
Container:
crntechnicalassessment-sqlserver


Port:
1433

The API connects to SQL Server using the Docker service name:

Server=sqlserver,1433

This is important because containers communicate using the Docker Compose service name.

19. Running the Application with Docker

Make sure Docker Desktop is running.

From the project root:

docker compose up --build

To run in detached mode:

docker compose up --build -d

Check running containers:

docker compose ps

Expected services:

crntechnicalassessment-api
crntechnicalassessment-sqlserver
20. Docker Database

The SQL Server database is:

CRNTechnicalAssessmentDb

The database is created and updated using Entity Framework Core migrations.

The SQL Server data is persisted through the Docker volume configured in:

docker-compose.yml
21. Checking the Docker Database

To connect to SQL Server inside the container:

docker exec crntechnicalassessment-sqlserver `
/opt/mssql-tools18/bin/sqlcmd `
-S localhost `
-U sa `
-P "YOUR_DATABASE_PASSWORD" `
-C `
-d CRNTechnicalAssessmentDb

Example query:

SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

To check products:

SELECT *
FROM Product;

To check items:

SELECT *
FROM Item;

To check users:

SELECT Id, Username, Role, IsActive
FROM Users;
22. Configuration

The application configuration is maintained through:

appsettings.json
appsettings.Development.json

Docker-specific configuration such as the SQL Server connection string is supplied through environment variables in:

docker-compose.yml

Sensitive credentials should not be hardcoded in production environments.

For production deployment, secrets should be provided through an appropriate secret-management mechanism or environment configuration.

23. Testing

The solution contains automated test projects.

tests/
│
├── CRNTechnicalAssessment.API.Tests
├── CRNTechnicalAssessment.Application.Tests
└── CRNTechnicalAssessment.Infrastructure.Tests

The testing stack includes:

xUnit
Moq
WebApplicationFactory

The API test project contains integration-style API tests.

The Application test project contains tests for application services and validators.

Tests can be executed using:

dotnet test
24. API Error Handling

The API follows standard HTTP status codes where applicable.

Common responses include:

Status Code	Meaning
200	Successful request
201	Resource created
204	Successful request with no response body
400	Invalid request
401	Authentication required/invalid
403	Access forbidden
404	Resource not found
500	Unexpected server error
25. Security

The application implements the following security mechanisms:

JWT authentication
Refresh token authentication
Password hashing
Role-based authorization
Input validation
Centralized exception handling
CORS configuration
HTTPS support through ASP.NET Core configuration
Database credentials supplied through configuration/environment variables

Passwords are not stored as plain text. Password hashes are stored in the Users table.

26. API Request Example
Create Product
POST /api/v1/Products

Example request:

{
  "productName": "Laptop",
  "items": [
    {
      "quantity": 10
    }
  ]
}

The exact request model is defined by the API DTOs and Swagger documentation.

27. Update Product
PUT /api/v1/Products/{id}

The product ID is provided through the URL and the updated product information is provided in the request body.

28. Delete Product
DELETE /api/v1/Products/{id}

The endpoint requires appropriate authorization.

29. API Documentation

Swagger/OpenAPI provides the primary interactive API documentation.

Swagger documents:

Available endpoints
HTTP methods
Request models
Response models
Authorization requirements
API versioning

Swagger can be used to execute and verify the APIs directly.

30. Project Execution Summary

The complete execution flow is:

Docker Compose
      │
      ├───────────────┐
      ▼               ▼
   API Container   SQL Server
      │               │
      │               │
      └───────┬───────┘
              │
              ▼
       Entity Framework Core
              │
              ▼
        Application Layer
              │
              ▼
          Domain Layer

For an API request:

Client
  │
  ▼
Swagger / HTTP Client
  │
  ▼
Controller
  │
  ▼
Application Service
  │
  ▼
Repository
  │
  ▼
Entity Framework Core
  │
  ▼
SQL Server
31. How to Run the Project
Prerequisites

Install:

.NET 8 SDK
Docker Desktop
Git
Clone the repository
git clone <repository-url>

Navigate to the project:

cd CRNTechnicalAssessment
Start the application
docker compose up --build
Verify containers
docker compose ps
Open Swagger
http://localhost:8080/swagger/index.html
32. Recommended Evaluation Flow

The API can be evaluated in the following order:

Start Docker Compose.
Verify API and SQL Server containers are running.
Open Swagger.
Execute the login API.
Copy the returned JWT access token.
Authorize Swagger using the JWT.
Test product GET operations.
Test product creation.
Test product update.
Test product deletion.
Test user-management endpoints according to authorization.
Test refresh-token functionality.
Review Swagger/OpenAPI documentation.
Review project structure and layered architecture.
33. Repository Structure

The repository contains:

CRNTechnicalAssessment/
│
├── CRNTechnicalAssessment.slnx
├── Dockerfile
├── docker-compose.yml
├── README.md
├── PROJECT_DOCUMENTATION.md
│
├── src/
│   ├── CRNTechnicalAssessment.API/
│   ├── CRNTechnicalAssessment.Application/
│   ├── CRNTechnicalAssessment.Domain/
│   └── CRNTechnicalAssessment.Infrastructure/
│
└── tests/
    ├── CRNTechnicalAssessment.API.Tests/
    ├── CRNTechnicalAssessment.Application.Tests/
    └── CRNTechnicalAssessment.Infrastructure.Tests/
34. Conclusion

The CRN Technical Assessment implements a RESTful Product Management API using ASP.NET Core Web API and .NET 8.

The solution demonstrates:

Layered architecture
RESTful API design
Product CRUD operations
JWT authentication
Refresh token authentication
Role-based authorization
Repository pattern
Entity Framework Core
SQL Server
FluentValidation
Centralized exception handling
Structured logging
Swagger/OpenAPI
Docker and Docker Compose
Automated testing

The solution is structured to keep API, application, domain, and infrastructure responsibilities separated while providing a maintainable and extensible backend architecture.
