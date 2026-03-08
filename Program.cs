using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Middleware;
using carkaashiv_angular_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddHealthChecks(); // Add health checks api came live and db
builder.Services.AddControllers();

//Business Services to be registered here
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPartService, PartService>();
builder.Services.AddScoped<EmployeeService>();




//AWS
var awsSection = builder.Configuration.GetSection("AWS");

var credentials = new BasicAWSCredentials(
    awsSection["AccessKey"],
    awsSection["SecretKey"]
);

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    return new AmazonS3Client(credentials,
        RegionEndpoint.GetBySystemName(awsSection["Region"]));
});
builder.Services.AddScoped<S3UploadServices>();
builder.Services.AddHttpContextAccessor(); //lets your service know:"Who is calling this API?"Without passing userId manually from controller




/*** Database connection block handles both prod and local **/

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(cs))
{
    throw new Exception("Connection string not found");
}

/*** JWT configuration ***/ 
var jwtSettings = builder.Configuration.GetSection("Jwt");

// Ensure jwtSettings["Key"] is not null before using it
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured. Please set 'Jwt:Key' in your configuration.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

    /**To extract token from cookie **/
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
                   .AllowAnyHeader() // for security remove it later
                   .AllowAnyMethod() // get,post,put,update
                   .AllowCredentials(); // important for cookies
    });
});
/*** Swagger ***/
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Carkaashiv 2.0 Api",
        Version="v1",
        Description = "Backend API for Carkaashiv 2.0 - ASP.NET core + PostgreSQL"
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";    
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

/*** Swagger page load first when starts ***/
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}else
{
    app.MapGet("/health", () => Results.Ok("Healthy"));
}
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();// Use authentication first & then authorization middleware
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/db");

app.Run();

