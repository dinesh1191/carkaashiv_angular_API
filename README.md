# CarKaashiv 2.0 – Backend API

Production-oriented ASP.NET Core Web API powering CarKaashiv 2.0, a scalable product ordering and order management platform. The API provides secure authentication, inventory management, customer ordering, payment verification, and order fulfillment workflows while following a layered architecture and modern backend development practices.

##  Project Highlights

* Layered Architecture (Controller → IService → Service → EF Core)
* JWT Authentication & Authorization
* Dockerized API
* Entity Framework Core with PostgreSQL
* CI/CD using GitHub Actions
* Production deployment on Render + Neon PostgreSQL
* Global Exception Middleware
* Idempotent Order Creation
* Standardized API Responses
* EF Core Migration Strategy

## System Overview
```text
Angular 19 Frontend
          │
          ▼
ASP.NET Core Web API
          │
Entity Framework Core
          │
PostgreSQL
```
---

## Production Stack

* API hosted on Render
* PostgreSQL hosted on Neon
* GitHub Actions for automated deployment
* Docker containerized backend

## Production Practices
* Environment-based configuration
* Secrets excluded from source control
* EF Core migration discipline
* Structured API responses
* Centralized exception handling
* Dockerized deployments
* CI/CD automation
* Health monitoring endpoint

## Tech Stack

* ASP.NET Core Web API
* C#
* Dependency Injection
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* Swagger/OpenAPI
* Docker
* GitHub Actions
* Health Checks
* Global Exception Middleware

---

## Architecture Overview

The API follows a layered architecture inspired by Clean Architecture principles:

```text
Controllers
    ↓
Service Interface (IService)
    ↓
Service Implementation (Service)
    ↓
ApplicationDbContext (EF Core)
    ↓
PostgreSQL
```
---

## Authentication & Security

### JWT Authentication

* Secure login using JWT access tokens
* Authenticated user validation through `/auth/me`
* Route protection using JWT authentication middleware
* Session-aware authentication flow
* Unauthorized requests return standardized responses

### Security Features

* Centralized authentication handling
* Protected administrative endpoints
* Environment-based configuration
* Secrets managed outside source control
* Standardized API error responses

---

## Core Business Modules

### User Management

* User registration
* Login
* Authenticated user validation
* Session management

### Product Management

* Create spare parts
* Update spare parts
* Delete spare parts
* Manage inventory availability
* Image upload support
* Fallback image handling

### Customer Ordering Workflow

* Shopping cart workflow
* Delivery information capture
* Order placement
* Invoice generation
* Payment proof submission
* Idempotent order creation support

### Order Fulfillment Workflow

```text
Submitted
      ↓
Verified
      ↓
ReadyForDispatch
      ↓
Shipped
```

Administrative actions include:

* Payment verification
* Dispatch processing
* Shipment tracking updates
* Order status management

---

## API Features

Interactive API documentation using Swagger/OpenAPI

### Standardized Responses

Consistent API response contracts for:

* Success responses
* Validation failures
* Business rule violations
* Server errors

### Validation

Server-side validation for:

* Customer information
* Delivery details
* Order placement requests
* Authentication requests

### Global Exception Handling

Centralized exception handling through custom middleware:

* Consistent error payloads
* Reduced controller boilerplate
* Improved diagnostics and maintainability

---

## Project Structure

```text
Controllers
Interfaces
Services
DTOs
Entities
Data
Middleware
Configurations
Program.cs
appsettings.json
appsettings.Development.json
```

---

## Database Strategy

### Development & Production

PostgreSQL is used as the primary database strategy.

Features:

* Entity Framework Core migrations
* Version-controlled schema changes
* Environment-specific configuration
* Production-ready cloud database deployment using Neon

Guidelines:

* No manual schema modifications
* Schema changes managed through EF Core migrations
* Migration history tracked in source control

---

## Example Endpoints

### Authentication
```text
POST   /api/auth/register
POST   /api/auth/login
GET    /api/auth/me
```
### Parts
```text
GET    /api/parts
GET    /api/parts/{id}
POST   /api/parts
PUT    /api/parts/{id}
DELETE /api/parts/{id}
```
### Orders
```text
POST   /api/orders/place-order
POST   /api/orders/upload-payment-proof
GET    /api/orders/submitted
GET    /api/orders/ready-for-dispatch
GET    /api/orders/shipped
```
### Health
```text
GET    /health/db
```
---

## Error Handling

Standard HTTP status code usage:

* 400 → Validation errors
* 401 → Unauthorized
* 403 → Forbidden
* 404 → Resource not found
* 409 → Business conflicts
* 500 → Unexpected server errors

Example:

```json
{
  "success": false,
  "message": "Payment proof already submitted.",
  "data": null
}
```

---

## Deployment & CI/CD

### Containerization

* Dockerized backend services
* Consistent deployment environments

### CI/CD

* GitHub Actions pipeline
* Automated build validation
* Automated deployment workflow

### Hosting

* Render (API Hosting)
* Neon PostgreSQL (Database)

---

## Local Setup

```bash
git clone <repository-url>
cd CarKaashiv.Api

dotnet restore
dotnet run
```

Trust HTTPS locally:

```bash
dotnet dev-certs https --trust
```

---

## Key Learnings Implemented

* JWT authentication and authorization
* Global exception middleware
* PostgreSQL production deployment
* Docker containerization
* CI/CD automation using GitHub Actions
* Idempotent API design
* Standardized API contracts
* Order lifecycle management
* Production-oriented database migration discipline

---
## Engineering Challenges Solved
* Designed a consistent service layer to keep controllers lightweight and business logic centralized.
* Prevented duplicate order creation using idempotency support.
* Centralized exception handling to eliminate repetitive controller logic.
* Standardized API responses for consistent frontend integration.
* Managed database schema evolution through EF Core migrations.
* Designed order lifecycle workflows supporting customer and administrative operations.

## Notes

* Frontend is implemented separately using Angular 19 (Standalone Architecture)
* API designed to support web and future mobile clients
* Business workflow driven by order lifecycle states
* Environment secrets are never committed to source control
* Database schema changes are managed exclusively through EF Core migrations

---

## Author

Dinesh Varadhan

Full Stack Developer (.NET + Angular)
