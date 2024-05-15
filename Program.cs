using Asp.Versioning;
using CatalogAPI.Context;
using CatalogAPI.DTOs.Mappings;
using CatalogAPI.Filters;
using CatalogAPI.Logging;
using CatalogAPI.Models;
using CatalogAPI.RateLimitOptions;
using CatalogAPI.Repositories;
using CatalogAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

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

// CORS Settings
builder.Services.AddCors(options =>
    options.AddPolicy("OriginsWithGrantedAccess",
        policy =>
        {
            policy.WithOrigins("https://localhost:7066")
                  .WithMethods("GET", "POST")
                  .AllowAnyHeader();
        })
);

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();
// When you are testing with a bearer token on Swagger, put 'Bearer in front of the token!!'
// Example: Bearer eyJhbGciOiJIUzI1NiIsIR5cCI6IkpXVCJ9...
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "catalogapi", Version = "v1" });
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "CatalogAPI",
        Description = "Catálogo de produtos e categorias",
        TermsOfService = new Uri("https://teste.net/terms"),
        Contact = new OpenApiContact
        {
            Name = "Gio",
            Email = "gio@mail.com",
            Url = new Uri("https://teste.net")
        },
        License = new OpenApiLicense
        {
            Name = "Usar sobre LICX",
            Url = new Uri("https://teste.net/license")
        }
    });

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey, // Use Http instead of ApiKey
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT",
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
            new string[] { }
        }
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().
    AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// builder.Services.AddAuthentication("Bearer").AddJwtBearer();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
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

// Policies
builder.Services.AddAuthorization(options =>
{
    // adds policy
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    // Only a specific user will be super admin
    options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireRole("Admin").RequireClaim("id", "Gio"));

    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

    options.AddPolicy("ExclusivePolicy", policy =>
        policy.RequireAssertion(context =>
        context.User.HasClaim(claim =>
            claim.Type == "id" && claim.Value == "Gio")
            || context.User.IsInRole("SuperAdmin")));
});

var rateLimitOptions = new MyRateLimitOptions();

builder.Configuration.GetSection(MyRateLimitOptions.RateLimit).Bind(rateLimitOptions);

builder.Services.AddRateLimiter(ratelimiteroptions =>
{
    ratelimiteroptions.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
    {
        options.PermitLimit = rateLimitOptions.PermitLimit; // 1;
        options.Window = TimeSpan.FromSeconds(rateLimitOptions.Window);
        options.QueueLimit = rateLimitOptions.QueueLimit;
        options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });
    ratelimiteroptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
                            RateLimitPartition.GetFixedWindowLimiter(
                                               partitionKey: httpcontext.User.Identity?.Name ??
                                                             httpcontext.Request.Headers.Host.ToString(),
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = true,
                                PermitLimit = 2,
                                QueueLimit = 0,
                                Window = TimeSpan.FromSeconds(1)
                            }));
});

// API Version Settings
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0); // Default version
    o.AssumeDefaultVersionWhenUnspecified = true; // If unspecified, use default
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine( // Versioning types used
        new QueryStringApiVersionReader(),
        new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
    // Swagger configs
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;   
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

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
// Middleware setup
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    //app.UseSwaggerUI();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CatalogAPI");
//    });
//    app.ConfigureExceptionHandler(); // Custom middleware
//}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseCors();

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
