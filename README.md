# CarKaashiv 2.0 – Backend API

Backend REST API for CarKaashiv 2.0, built with ASP.NET Core and designed using a decoupled architecture to support Angular frontend clients. The platform enables spare parts catalog management, customer ordering, payment proof submission, and order fulfillment workflows.

## Tech Stack

* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* Repository Pattern
* Docker
* Swagger / OpenAPI
* GitHub Actions CI/CD
* Render Deployment
* Neon PostgreSQL (Production)

---

## Architecture Overview

The API follows a layered architecture with clear separation of concerns:

```text
Controllers
    ↓
Services
    ↓
Repositories
    ↓
Entity Framework Core
    ↓
PostgreSQL
```

Key goals:

* Maintainability
* Testability
* Scalability
* Clear separation of business logic and data access

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

### Parts Management

* Create spare parts
* Update spare parts
* Delete spare parts
* Manage inventory availability
* Image upload support
* Fallback image handling

### Customer Ordering

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
Services
Repositories
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

POST   /api/auth/register
POST   /api/auth/login
GET    /api/auth/me

### Parts

GET    /api/parts
GET    /api/parts/{id}
POST   /api/parts
PUT    /api/parts/{id}
DELETE /api/parts/{id}

### Orders

POST   /api/orders/place-order
POST   /api/orders/upload-payment-proof
GET    /api/orders/submitted
GET    /api/orders/ready-for-dispatch
GET    /api/orders/shipped

### Health

GET    /health/db

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

* Repository Pattern implementation
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
