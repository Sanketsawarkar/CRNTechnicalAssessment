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