using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddHealthChecks(); // Add health checks api came live and db
builder.Services.AddControllers();


/***Database connection handles both prod and local**/
builder.Services.AddScoped<IAuthService, AuthService>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (builder.Environment.IsDevelopment())
{
    /***points local developement -> SQL server **/
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
     //  options.UseSqlServer(connectionString);//uncomment when pointing local dev mssql server
        options.UseNpgsql(connectionString); // uncomment when pointing prod neon server
    });  
}
else
{
    /***points Production(Redner) -> PostgreSQL**/
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(connectionString);      

    });
}

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(cs))
{
    throw new Exception("Connection string not found");
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    


    };

    // to extract token from cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        
        {
            var logger = context.HttpContext.RequestServices
               .GetRequiredService<ILoggerFactory>()
               .CreateLogger("JWT");
            //Console.WriteLine("JWT VALID Issuer: " + jwtSettings["Issuer"]);
            //Console.WriteLine("JWT VALID Audience: " + jwtSettings["Audience"]);
            //Console.WriteLine("JWT VALID Key Len: " + jwtKey?.Length);

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
                   .AllowAnyHeader() // for security remove it later
                   .AllowAnyMethod() // get,post,put,update
                   .AllowCredentials(); // important for cookies
    });
});



var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();// Use authentication first & then authorization middleware
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/db");

app.Run();

