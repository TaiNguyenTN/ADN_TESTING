
using System.Text;
using Application.Interface;
using Application.Mapper;
using Application.Services;
using Domain.IRepository;
using DotNetEnv;
using IAmService.API.gRPC;
using Infrastructure.Data;
using Infrastructure.GrpcClients;
using Infrastructure.Repository;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

Env.Load();
var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    // REST API + gRPC
    options.ListenAnyIP(5010, o =>
    {
        o.Protocols = HttpProtocols.Http1;
    });

    // gRPC only
    options.ListenAnyIP(5011, o =>
    {
        o.Protocols = HttpProtocols.Http2;
    });
});

// Add services to the container.

builder.Services.AddControllers();

// Add gRPC support
builder.Services.AddGrpc();


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Booking Service", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter token"
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
            new string[] {}
        }
    });
});

//JWT
//var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
//var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
//var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters()
//        {
//            ValidateIssuer = true,
//            ValidIssuer = jwtIssuer,
//            ValidateAudience = true,
//            ValidAudience = jwtAudience,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
//        };
//    });

//Mastransit RabbitMQ
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.UsingRabbitMq((context, configuratior) =>
    {
        var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        var vHost = Environment.GetEnvironmentVariable("RABBITMQ_VHOST");
        var username = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER");
        var password = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS");
        configuratior.Host(host, vHost, h =>
        {
            h.Username(username!);
            h.Password(password!);
        });
    });
});

//Connection with Database
var connectionString = Environment.GetEnvironmentVariable("BOOKING_DB_CONNECTION");
builder.Services.AddDbContext<BookDbContext>(options =>
    options.UseSqlServer(connectionString));

//Mapper
builder.Services.AddAutoMapper(typeof(AppointmentMappingProfile));

//DI Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<ITestPurposeRepository, TestPurposeRepository>();
builder.Services.AddScoped<ITestCategoryRepository, TestCategoryRepository>();
builder.Services.AddScoped<IUserGrpcClient, UserGrpcClient>();


//DI Service
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IADNService, ADNService>();
builder.Services.AddScoped<ITestCategoryService, TestCategoryService>();

var app = builder.Build();

//Auto migrate database
using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookDbContext>();

    var retries = 5;
    for(int i = 0; i < retries; i++)
    {
        try
        {
            await db.Database.MigrateAsync();
            await Seeder.SeedASync(db);
            Console.WriteLine("Database migrated and seed successfully");
            break;
        }
        catch (SqlException)
        {
            Console.WriteLine($"Database not ready, retrying in 5s... ({i + 1}/{retries})");
            await Task.Delay(5000);
            if (i == retries - 1) throw;
        }
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
