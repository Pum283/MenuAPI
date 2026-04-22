using Infrastructure.Persistence;
using API.Middlewares;
using Application;
using Infrastructure;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Media;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

static string? BuildConnectionStringFromEnv()
{
    // Prefer explicit full connection string if provided.
    var raw = Environment.GetEnvironmentVariable("MENUAPI_CONNECTION_STRING");
    if (!string.IsNullOrWhiteSpace(raw))
    {
        return raw.Trim();
    }

    // Otherwise build from individual environment variables.
    var server = Environment.GetEnvironmentVariable("MENUAPI_DB_SERVER");
    var database = Environment.GetEnvironmentVariable("MENUAPI_DB_NAME");
    var userId = Environment.GetEnvironmentVariable("MENUAPI_DB_USER");
    var password = Environment.GetEnvironmentVariable("MENUAPI_DB_PASSWORD");

    if (string.IsNullOrWhiteSpace(server) ||
        string.IsNullOrWhiteSpace(database) ||
        string.IsNullOrWhiteSpace(userId) ||
        string.IsNullOrWhiteSpace(password))
    {
        return null;
    }

    // Keep this in sync with your infra requirements; avoid logging secrets.
    return $"Server={server};Database={database};User Id={userId};Password={password};Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";
}

var envConnectionString = BuildConnectionStringFromEnv();
if (!string.IsNullOrWhiteSpace(envConnectionString))
{
    // Override ConnectionStrings:DefaultConnection so Infrastructure picks it up.
    builder.Configuration["ConnectionStrings:DefaultConnection"] = envConnectionString;
}

// Options from env
var jwtSigningKey = Environment.GetEnvironmentVariable("MENUAPI_JWT_SIGNING_KEY");
if (!string.IsNullOrWhiteSpace(jwtSigningKey))
{
    builder.Configuration["Jwt:SigningKey"] = jwtSigningKey;
}

var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
if (!string.IsNullOrWhiteSpace(cloudinaryUrl))
{
    builder.Configuration["Cloudinary:CloudinaryUrl"] = cloudinaryUrl;
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "MenuAPI", 
        Version = "v1" 
    });
});

// Add Application Layer
builder.Services.AddApplication();

// Add Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CloudinaryOptions>(builder.Configuration.GetSection("Cloudinary"));

var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSection.GetValue<string>("SigningKey") ?? string.Empty;
if (string.IsNullOrWhiteSpace(signingKey))
{
    // Keep app bootable for local InMemory; protected endpoints will still require auth.
    // In production you must set MENUAPI_JWT_SIGNING_KEY.
    signingKey = "DEV_ONLY_CHANGE_ME_32_BYTES_MINIMUM_KEY!!";
    builder.Configuration["Jwt:SigningKey"] = signingKey;
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection.GetValue<string>("Issuer"),
            ValidAudience = jwtSection.GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Exception handling middleware
app.UseExceptionMiddleware();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
        }
        else
        {
            context.Database.EnsureCreated();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

app.Run();
