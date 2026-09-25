using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealEstateAiAgent.Api.Configuration;
using RealEstateAiAgent.Api.Data;
using RealEstateAiAgent.Api.ExceptionHandling;
using RealEstateAiAgent.Api.Models;
using RealEstateAiAgent.Api.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting RealEstateAiAgent.Api");

    var builder = WebApplication.CreateBuilder(args);

    // --- Logging (skip in integration tests) ---
    if (!builder.Environment.IsEnvironment("Testing"))
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
    }

    // --- MVC / API ---
    builder.Services.AddControllers();
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("FrontendDev", policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // --- Data ---
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        if (builder.Environment.IsEnvironment("Testing"))
        {
            options.UseInMemoryDatabase("RealEstateAiAgentTests");
        }
        else
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        }
    });

    // --- Auth (JWT) ---
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

    var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
        ?? throw new InvalidOperationException("Jwt configuration is missing.");

    var jwtSigningKey = jwtOptions.Key;
    if (string.IsNullOrWhiteSpace(jwtSigningKey))
    {
        if (builder.Environment.IsEnvironment("Testing"))
        {
            jwtSigningKey = "test-secret-key-at-least-32-characters-long!!";
            builder.Services.PostConfigure<JwtOptions>(options => options.Key = jwtSigningKey);
        }
        else
        {
            throw new InvalidOperationException("Jwt:Key is not configured. Use User Secrets for local dev.");
        }
    }

    builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
    builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    builder.Services.AddScoped<IPropertySearchService, PropertySearchService>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
                ClockSkew = TimeSpan.FromMinutes(1),
            };
        });

    builder.Services.AddAuthorization();
    builder.Services.AddMemoryCache();

    // --- Bedrock / Semantic Kernel (not needed for integration tests) ---
    if (!builder.Environment.IsEnvironment("Testing"))
    {
        builder.Services.AddBedrockAi(builder.Configuration);
        builder.Services.AddScoped<INaturalLanguagePropertySearchService, NaturalLanguagePropertySearchService>();
    }

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // --- Middleware pipeline ---
    app.UseExceptionHandler();

    if (!app.Environment.IsEnvironment("Testing"))
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
            };
        });
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseCors("FrontendDev");

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await PropertySeeder.SeedAsync(db);

        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
