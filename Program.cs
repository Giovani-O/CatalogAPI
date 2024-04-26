using CatalogAPI.Context;
using CatalogAPI.Filters;
using CatalogAPI.Logging;
using CatalogAPI.Models;
using CatalogAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Ignoring cyclic references that happen when using navigation properties
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter)); // Using a global filter
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
var valor1 = builder.Configuration["Key1"];

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddScoped<ApiLoggingFilter>(); // Registering the filter service

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Generic repository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // Registering the Category repository service
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Never forget to register the repository -_-
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Register unit of work

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

// Create the application
var app = builder.Build();

// Configure the HTTP request pipeline.
// Middleware setup
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler(); // Custom middleware
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Creation of a custom middleware
// app.Use(async (context, next) => {
//     // Add code before request
//     await next(context);
//     // Add code after request
// });

app.MapControllers();

// Creation of a final middleware
// app.Run(async (context) =>
// {
//     await context.Response.WriteAsync("Middleware final");
// });

app.Run();
