using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHealthChecks(); // Add health checks api came live and db



// Database connection
builder.Services.AddScoped<IAuthService, AuthService>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (builder.Environment.IsDevelopment())
{
    //local developement -> SQL server
    builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseSqlServer(connectionString));
       
}
else
{
    //Production(Redner) -> PostgreSQL
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(connectionString);
    });
}




// JWT configuration 
var jwtSettings = builder.Configuration.GetSection("Jwt");

// Ensure jwtSettings["Key"] is not null before using it
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured. Please set 'Jwt:Key' in your configuration.");
}

builder.Services.AddAuthentication(options =>
{
    //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
    // to extract token from cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        
        {
            var logger = context.HttpContext.RequestServices
               .GetRequiredService<ILoggerFactory>()
               .CreateLogger("JWT");

            if (context.Request.Cookies.ContainsKey("jwtToken"))
            {
                context.Token = context.Request.Cookies["jwtToken"];
                logger.LogInformation("JWT cookie found. Token: {token}", context.Token?.Substring(0, 20) + "...");
            }
            else
            {
                logger.LogWarning("JWT cookie not found in request.");
            }
            return Task.CompletedTask;
        }

    };
});



//Enable CORS(for angular app)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200", // local Angular app frontend URL
                 "https://dulcet-rolypoly-2f31fc.netlify.app") // production frontend domain  url
                   .AllowAnyHeader()
                   .AllowAnyMethod() // get,post,put,update
                   .AllowCredentials(); // important for cookies
    });
});
// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();// Use authentication first & then authorization middleware
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/db");

app.Run();

