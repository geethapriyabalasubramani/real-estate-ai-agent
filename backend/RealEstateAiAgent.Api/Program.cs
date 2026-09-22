using Microsoft.EntityFrameworkCore;
using RealEstateAiAgent.Api.Data;
using RealEstateAiAgent.Api.ExceptionHandling;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting RealEstateAiAgent.Api");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddControllers();
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

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseExceptionHandler();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
        };
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseCors("FrontendDev");

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await PropertySeeder.SeedAsync(db);

        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
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