# CRN Technical Assessment

## Project Overview

This project is a Product Management REST API developed using ASP.NET Core Web API.

The application provides APIs for:

- User authentication
- JWT-based authorization
- Refresh token authentication
- Product CRUD operations
- Request validation
- Centralized exception handling
- Structured logging
- API versioning
- Role-based authorization

The solution follows a layered architecture to improve maintainability, scalability, and separation of concerns.

---

## Technologies Used

- .NET 8
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- Refresh Tokens
- FluentValidation
- Serilog
- Swagger / OpenAPI
- Docker
- Docker Compose

---

## Architecture

The project follows a layered architecture:

```text
CRNTechnicalAssessment
│
├── src
│   ├── CRNTechnicalAssessment.API
│   │   ├── Controllers
│   │   ├── Middleware
│   │   └── Program.cs
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
    └── Test projects


## API Endpoints

### Authentication

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/api/v1/Auth/login` | Authenticate user and generate JWT tokens | Public |
| POST | `/api/v1/Auth/refresh` | Generate a new access token using refresh token | Public |

### Products

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/v1/Products` | Get all products | Authenticated |
| GET | `/api/v1/Products/{id}` | Get product by ID | Authenticated |
| POST | `/api/v1/Products` | Create a new product | Admin |
| PUT | `/api/v1/Products/{id}` | Update an existing product | Admin |
| DELETE | `/api/v1/Products/{id}` | Delete a product | Admin |

### Users

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/v1/Users` | Get users | Admin |
| POST | `/api/v1/Users` | Create a user | Admin |

---

## Authentication Flow

The API uses JWT-based authentication with refresh tokens.

1. User sends username and password to the login endpoint.
2. The API validates the user credentials.
3. A short-lived JWT access token is generated.
4. A refresh token is generated and stored in the database.
5. The client uses the access token to access protected endpoints.
6. When the access token expires, the refresh token can be used to obtain a new access token.
7. Role-based authorization controls access to protected operations.

Example login request:

```json
{
  "username": "admin",
  "password": "Admin@123"
}
