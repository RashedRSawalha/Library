using Kernal;
using Shared.Helpers;
using Shared.Contracts;
using LibraryManagementInfrastructure;
using Persistence.Interface;
using Persistence.UnitOfWork;
using Persistence.Repository;
using LibraryManagementApplication.Validation;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Shared.Redis;
using LibraryManagementAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LibraryManagementApplication.Authentication;
using LibraryManagementAPI.Middlewares;
using Microsoft.OpenApi.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using LibraryManagementDomain.DTO;
using Scrutor;
using LibraryManagementApplication.Commands;
using LibraryManagementApplication.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Determine the database provider from configuration
var databaseProvider = builder.Configuration["DatabaseProvider"];
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(databaseProvider) || string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DatabaseProvider or ConnectionString is not configured properly.");
}

// Register LibraryDBContext based on the selected database provider
builder.Services.AddDbContext<LibraryDBContext>(options =>
{
    if (databaseProvider == "PostgreSQL")
    {
        options.UseNpgsql(connectionString)
               .EnableSensitiveDataLogging() // Debugging only
               .LogTo(Console.WriteLine);  // Logs SQL queries
    }
    else if (databaseProvider == "SqlServer")
    {
        options.UseSqlServer(connectionString, b => b.MigrationsAssembly("LibraryManagementInfrastructure"));
    }
    else
    {
        throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}");
    }
});

// Register MediatR
builder.Services.AddMediatR(configuration =>
{
    // Register the assembly containing handlers
    configuration.RegisterServicesFromAssembly(typeof(LibraryManagementApplication.Queries.GetAuthorsQuery).Assembly);
});

//Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Capture Debug and higher-level logs
    .Enrich.FromLogContext()
    .WriteTo.Console() // Log to Console
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Log to File
    .CreateLogger();

//Configure RedisCache using IDistributedCache (StackExchange.Redis)
builder.Services.AddStackExchangeRedisCache(options =>
{
   options.Configuration = builder.Configuration.GetConnectionString("Redis"); // Replace with your Redis connection string
   options.InstanceName = "RedisCacheInstance"; // Optional: specify an instance name
});



//Register Serilog as the logging provider
builder.Host.UseSerilog();

// Register IRedisCache implementation
builder.Services.AddSingleton<IRedisCache, RedisCache>();

// Register HttpClient
builder.Services.AddHttpClient();

// Register UnitOfWork and Generic Repository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register RabbitMQ Sender and Receiver
builder.Services.AddSingleton(typeof(IRabbitMQSender<>), typeof(RabbitMQSender<>));
builder.Services.AddSingleton(typeof(IRabbitMQReceiver<>), typeof(RabbitMQReceiver<>));


// Register Commands and Queries using Scrutor
builder.Services.Scan(scan =>
    scan.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
        .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
        .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

// RabbitMQ dependencies
builder.Services.AddSingleton(sp =>
{
    return new ConnectionFactory
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
    };
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer <token>"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Optional: Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddSingleton<TokenService>();
builder.Services.AddFluentValidationAutoValidation();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddControllersWithViews()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AuthorValidator>());

builder.Services.AddAuthorization();

builder.Services.AddScoped<Dispatcher>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<JwtMiddleware>();
app.UseAuthorization();

// Optional: Apply CORS
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
