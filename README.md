**CarKaashiv 2.0 – API**

Backend REST API for CarKaashiv 2.0, built with ASP.NET Core and designed using a decoupled architecture to support Angular frontend clients.

**Tech Stack**

ASP.NET Core Web API

Entity Framework Core

JWT Authentication (Cookie-based)

MSSQL (Local Development)

PostgreSQL (Neon – Production)

CORS enabled for Angular frontend

 **Authentication**

JWT token generated on login

Token stored securely in HttpOnly cookies

auth/me endpoint validates authenticated users

Cookie-based auth supports Angular withCredentials

📂 **Project Structure**
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

**Database Strategy**

Local Development: MSSQL

Production: Neon PostgreSQL

Database changes are tracked using versioned SQL migration scripts

No manual schema changes in production

**Example migration folders**
db/migrations/mssql
db/migrations/postgresql

Environment Configuration
Development (Local MSSQL)
"ConnectionStrings": {
  "DefaultConnection": "Server=DESKTOP-XXXX;Database=car_kaashiv_web_app;Trusted_Connection=True;TrustServerCertificate=True"
}

Production (Neon PostgreSQL)

Configured via Render environment secrets.

 **API Endpoints (Sample)**
Method	Endpoint	Description
POST	/api/auth/register	Register new user
POST	/api/auth/login	Login user
GET	/api/auth/me	Get logged-in user
GET	/health/db	DB health check

 **Error Handling**

400 → Validation errors

401 → Unauthorized

409 → Duplicate / Conflict (eg: phone already registered)

Consistent response format:

{
  "success": false,
  "message": "Mobile number already registered.",
  "data": null
}

 **Local Setup**
git clone <repo-url>
cd CarKaashiv.Api
dotnet restore
dotnet run


**Trust HTTPS locally:**

dotnet dev-certs https --trust

**Key Learnings Implemented**

Proper HTTP status codes (409 Conflict)

DB-driven timestamps (DEFAULT CURRENT_TIMESTAMP / SYSDATETIME)

JWT cookie handling with Angular

Environment-safe DB configuration

DB migration discipline (local → prod)

**Notes**

Frontend handled separately (Angular)

Snackbar used for frontend notifications

DB is versioned via migration scripts, not manual edits

 **Author**

Dinesh Varadhan
Full Stack Developer (.NET + Angular)
