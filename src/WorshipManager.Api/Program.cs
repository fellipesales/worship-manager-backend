using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WorshipManager.Application;
using WorshipManager.Core.Entities;
using WorshipManager.Infrastructure;
using WorshipManager.Infrastructure.Data;
using WorshipManager.Infrastructure.Identity;
using WorshipManager.Infrastructure.Middleware;
using WorshipManager.Infrastructure.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Validate required configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found. " +
        "Please configure it via environment variable 'ConnectionStrings__DefaultConnection' " +
        "or in appsettings.json");
}

// Add Infrastructure and Application layers
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>();

// Configure JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "WorshipManager";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "WorshipManagerApp";
var key = Encoding.UTF8.GetBytes(jwtSecret);

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Google OAuth (if credentials are provided)
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
            options.SaveTokens = true;
            options.Scope.Add("profile");
            options.Scope.Add("email");
        });
}

// Configure Facebook OAuth (if credentials are provided)
var facebookAppId = builder.Configuration["Authentication:Facebook:AppId"];
var facebookAppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
if (!string.IsNullOrEmpty(facebookAppId) && !string.IsNullOrEmpty(facebookAppSecret))
{
    builder.Services.AddAuthentication()
        .AddFacebook(options =>
        {
            options.AppId = facebookAppId;
            options.AppSecret = facebookAppSecret;
            options.SaveTokens = true;
        });
}

// Configure Microsoft OAuth (if credentials are provided)
var microsoftClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
var microsoftClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
if (!string.IsNullOrEmpty(microsoftClientId) && !string.IsNullOrEmpty(microsoftClientSecret))
{
    builder.Services.AddAuthentication()
        .AddMicrosoftAccount(options =>
        {
            options.ClientId = microsoftClientId;
            options.ClientSecret = microsoftClientSecret;
            options.SaveTokens = true;
        });
}

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsOrgAdmin", policy =>
        policy.RequireClaim("OrganizationRole", "Admin"));

    options.AddPolicy("IsOrgLeaderOrAdmin", policy =>
        policy.RequireClaim("OrganizationRole", "Leader", "Admin"));

    options.AddPolicy("IsOrgMember", policy =>
        policy.RequireAuthenticatedUser());
});

// CORS
var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? ["http://localhost:3000"];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Global exception handler
app.UseGlobalExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Apply database migrations via DbUp
try
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Running database migrations...");
    DatabaseMigrator.Migrate(connectionString, logger);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Failed to apply database migrations");
    throw;
}

// CORS before auth
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Multi-tenant middleware (needs authenticated user)
app.UseTenantMiddleware();

// Health check
app.MapGet("/health", async (ApplicationDbContext? dbContext, IConfiguration config) =>
{
    var checks = new Dictionary<string, object>
    {
        ["timestamp"] = DateTime.UtcNow,
        ["environment"] = app.Environment.EnvironmentName,
        ["connectionStringConfigured"] = !string.IsNullOrEmpty(config.GetConnectionString("DefaultConnection"))
    };

    try
    {
        if (dbContext != null)
        {
            await dbContext.Database.CanConnectAsync();
            checks["database"] = "connected";
        }
    }
    catch (Exception ex)
    {
        checks["database"] = $"error: {ex.Message}";
    }

    return Results.Json(checks);
});

app.MapControllers();

app.Run();
