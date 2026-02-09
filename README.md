# CarKaashiv 2.0 – API

Backend REST API for CarKaashiv 2.0, built with ASP.NET Core and designed using a decoupled architecture to support Angular frontend clients.

---

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- JWT Authentication using HttpOnly Cookies
- MSSQL (Local Development)
- PostgreSQL (Neon – Production)
- CORS enabled for Angular frontend

---

## Architecture Overview

The API follows a layered architecture with clear separation of concerns using Controllers, Services, Repositories, and DTOs to ensure maintainability and scalability.

---

## Authentication

- JWT token generated on successful login
- Token stored securely in **HttpOnly cookies**
- `auth/me` endpoint validates authenticated users
- Cookie-based JWT authentication supports Angular `withCredentials`

---

## Project Structure

```
├── Controllers
├── Services
├── DTOs
├── Entities
├── Data
├── db
│   └── migrations
│       ├── mssql
│       └── postgresql
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

---

## Database Strategy

- **Local Development:** MSSQL
- **Production:** Neon PostgreSQL (Serverless)

Database schema is managed using **Entity Framework Core**, with database-specific migration scripts versioned separately for MSSQL and PostgreSQL.

- No manual schema changes in production
- All changes are tracked via migration scripts

Example migration folders:
- `db/migrations/mssql`
- `db/migrations/postgresql`

---

## Environment Configuration

### Development (Local MSSQL)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-XXXX;Database=car_kaashiv_web_app;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Production (Neon PostgreSQL)

Configured using Render environment secrets

**No credentials committed to source control**

---

## API Endpoints (Sample)

| Method | Endpoint           | Description                    |
|--------|-------------------|--------------------------------|
| POST   | /api/auth/register | Register new user              |
| POST   | /api/auth/login    | Login user                     |
| GET    | /api/auth/me       | Get logged-in user             |
| GET    | /health/db         | Database health check          |

---

## Error Handling

The API uses consistent HTTP status codes and a unified response format.

- **400** → Validation and bad request errors
- **401** → Unauthorized access
- **409** → Conflict (eg: mobile number already registered)
- **500** → Internal server errors (unexpected failures)

### Error Response Format

```json
{
  "success": false,
  "message": "Mobile number already registered.",
  "data": null
}
```

---

## Deployment & CI/CD

- Backend is containerized using Docker
- Deployed on Render
- Automated build and deployment using GitHub Actions

---

## Local Setup

```bash
git clone <repository-url>
cd CarKaashiv.Api
dotnet restore
dotnet run
```

### Trust HTTPS locally

```bash
dotnet dev-certs https --trust
```

---

## Key Learnings Implemented

- Proper HTTP status code usage (409 Conflict)
- DB-driven timestamps (CURRENT_TIMESTAMP, SYSDATETIME)
- Secure JWT cookie handling with Angular
- Environment-based configuration management
- Strict database migration discipline (local → production)

---

## Notes

- Frontend is handled separately using Angular
- Snackbar is used for frontend notifications
- Database schema is versioned via migration scripts only
- Sensitive configuration values (DB credentials, JWT secrets) are managed using environment variables and are not committed to source control

---

## Author

**Dinesh Varadhan**  
Full Stack Developer (.NET + Angular)