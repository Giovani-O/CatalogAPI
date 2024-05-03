using CatalogAPI.Context;
using CatalogAPI.DTOs.Mappings;
using CatalogAPI.Filters;
using CatalogAPI.Logging;
using CatalogAPI.Models;
using CatalogAPI.Repositories;
using CatalogAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
}).AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().
    AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication("Bearer").AddJwtBearer();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
var valor1 = builder.Configuration["Key1"];

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

// Configuring JWT ///////////////////////////////////////////////////////////////////

// Gets the secret key from appsettings.json
var secretKey = builder.Configuration["JWT:SecretKey"]
    ?? throw new ArgumentException("Invalid secret key!");

// JWT service configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true; // Save the token
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    { 
        // Token generation parameters
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});

// ///////////////////////////////////////////////////////////////////////////////////


builder.Services.AddScoped<ApiLoggingFilter>(); // Registering the filter service

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Generic repository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // Registering the Category repository service
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Never forget to register the repository -_-
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Register unit of work
builder.Services.AddScoped<ITokenService, TokenService>(); // Register token service

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddAutoMapper(typeof(ProductDTOMappingProfile));

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
