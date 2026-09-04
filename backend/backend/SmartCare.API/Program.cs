using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SmartCare.Application;
using SmartCare.Application.Common.DTOs;
using SmartCare.API.Auth;
using SmartCare.Infrastructure;
using SmartCare.API.Middleware;
using SmartCare.API.Health;
using SmartCare.API.Services;
using SmartCare.Infrastructure.Persistence;
using Serilog;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = ApiResponse.ErrorResult("Validation failed", errors);
            return new BadRequestObjectResult(response);
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartCare API",
        Version = "v1",
        Description = "SmartCare Healthcare Management System API",
        Contact = new OpenApiContact
        {
            Name = "SmartCare Team",
            Email = "support@smartcare.com"
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.AddSingleton<ITokenService, JwtTokenService>();
builder.Services.AddScoped<SmartCare.API.Services.IEmailService, SmartCare.API.Services.EmailService>();

// Rate limiting: protect auth endpoints from brute-force and credential stuffing
builder.Services.AddRateLimiter(options =>
{
    // Login: max 20 attempts per IP per minute
    options.AddSlidingWindowLimiter("auth-login", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.SegmentsPerWindow = 6;
        limiterOptions.PermitLimit = 20;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Register: max 20 new accounts per IP per hour
    options.AddSlidingWindowLimiter("auth-register", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromHours(1);
        limiterOptions.SegmentsPerWindow = 6;
        limiterOptions.PermitLimit = 20;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Return 429 with a Retry-After header
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers.RetryAfter = "60";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { success = false, message = "Too many requests. Please try again later." },
            cancellationToken);
    };
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are missing.");

        // Prevent ASP.NET from remapping short claim names (e.g. "role") to long URI forms
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = "role",
            NameClaimType = "sub"
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError($"JWT Authentication failed: {context.Exception?.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var claims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}").ToList() ?? new List<string>();
                logger.LogInformation($"JWT Token validated. Claims: {string.Join(", ", claims)}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning($"JWT Challenge triggered. Reason: {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewDashboard", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor, AppRoles.Nurse, AppRoles.Patient));
    options.AddPolicy("ViewDoctors", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor, AppRoles.Nurse, AppRoles.Patient));
    options.AddPolicy("ManageDoctors", policy => policy.RequireRole(AppRoles.Admin));
    options.AddPolicy("DeleteDoctors", policy => policy.RequireRole(AppRoles.Admin));
    options.AddPolicy("ViewAppointments", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor, AppRoles.Nurse, AppRoles.Patient));
    options.AddPolicy("ManageAppointments", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor, AppRoles.Nurse));
    options.AddPolicy("DeleteAppointments", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor));
    options.AddPolicy("ViewMedicalRecords", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor, AppRoles.Nurse, AppRoles.Patient));
    options.AddPolicy("ManageMedicalRecords", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor, AppRoles.Nurse));
    options.AddPolicy("DeleteMedicalRecords", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor));
    options.AddPolicy("ViewPatientDetails", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor, AppRoles.Nurse, AppRoles.Patient));
    options.AddPolicy("ViewPatientList", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor, AppRoles.Nurse));
    options.AddPolicy("ManagePatients", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor, AppRoles.Nurse));
    options.AddPolicy("DeletePatients", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Doctor));
});

// Add application and infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<SmartCareDbContextHealthCheck>("SmartCareDbContext");

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartCare API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger at root
    });
}

// Use custom middleware
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseCors("AllowFrontend");

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Add health check endpoint
app.MapHealthChecks("/health");

// Initialize database
try
{
    await app.Services.InitializeDatabaseAsync();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<SmartCare.Infrastructure.Persistence.SmartCareDbContext>();
        var adminUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == "admin@smartcare.local" || u.Role.ToLower() == AppRoles.Admin.ToLower());
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<SmartCare.Infrastructure.Persistence.Entities.UserEntity>();

        if (adminUser == null)
        {
            var bootstrapUser = new SmartCare.Infrastructure.Persistence.Entities.UserEntity
            {
                Id = Guid.NewGuid(),
                Email = "admin@smartcare.local",
                Role = AppRoles.Admin,
                CreatedAt = DateTime.UtcNow
            };
            bootstrapUser.PasswordHash = hasher.HashPassword(bootstrapUser, "Admin@123");
            dbContext.Users.Add(bootstrapUser);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            adminUser.Role = AppRoles.Admin;
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");
            await dbContext.SaveChangesAsync();
        }
    }

    app.Logger.LogInformation("Database initialized successfully");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "An error occurred while initializing the database");
}

app.Logger.LogInformation("SmartCare API started successfully");

app.Run();
