using LegacyDataAccess.Core.Interfaces;
using LegacyDataAccess.Dapper.Configuration;
using LegacyDataAccess.Dapper.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Dapper configuration
builder.Services.AddSingleton<DapperConfiguration>();

// Add repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();

// Add health checks
builder.Services.AddHealthChecks();

// Add CORS for legacy clients
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health");
app.MapGet("/health/ready", () => Results.Ok(new { status = "ready", timestamp = DateTime.UtcNow }));
app.MapGet("/health/live", () => Results.Ok(new { status = "live", timestamp = DateTime.UtcNow }));

app.Run();