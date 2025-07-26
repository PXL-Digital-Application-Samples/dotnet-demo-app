using UserManagementApi.Core.Interfaces;
using UserManagementApi.Core.Services;
using UserManagementApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "User Management API", 
        Version = "v1",
        Description = "A simple API for managing users with CRUD operations"
    });
    
    // Enable XML comments for better API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register our services
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for demo purposes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API V1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at root
});

// Only use HTTPS redirection if HTTPS is properly configured
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

// Configure the port - use environment variable PORT, command line --urls, or default to 5000
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
if (string.IsNullOrEmpty(builder.Configuration["urls"]) && !args.Any(arg => arg.StartsWith("--urls")))
{
    app.Urls.Add($"http://localhost:{port}");
}

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
