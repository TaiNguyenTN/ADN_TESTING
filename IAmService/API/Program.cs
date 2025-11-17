

using System.Text;
using Application.Interfaces;
using Application.Services;
using Domain.IRepositories;
using DotNetEnv;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "I Am Service", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //Định nghĩa cơ chế xác thực(security scheme) cho swagger - ở đây là JWT Bearer 
    //Token
    {
        Name = "Authorization", //Tên header dung để gửi token
        Type = SecuritySchemeType.Http, //Kiểu xác thực là HTTP(vd:Basic, Bearer,...)
        Scheme = "bearer",  //Đặt scheme là bearer(chuẩn JWT token).
        BearerFormat = "JWT",   //Xác định form token là JWT
        In = ParameterLocation.Header,  //Token sẽ được gửi trong header HTTP
        Description = "Enter token" //Mô tả hiển thị cho người dùng trong Swagger UI
    });
    //=> Kết quả là: Trong Swagger UI, bạn sẽ thấy nút "Authorize", khi bấm vào thì
    //sẽ có ô nhập token, ví dụ:
    //Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...


    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //Xác định rằng tât cả các endponint trong API đều yêu cầu xác thực "Bearer" mà
    //bạn vừa định nghĩa ở trên. Nếu không thêm phần này, Swagger sẽ chỉ hiện nút
    //"Authorize" nhưng không áp dụng token khi gọi thử các endpoint
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
    //=> Có nghĩa là: Swagger, hãy áp dụng security scheme có ID là Bearer(định nghĩa
    //ở trên) cho tất cả request mặc định.

    /*Tóm lại
    1. AddSwaggerGen: Đăng ký Swagger generator để sinh tài liệu API.
    2. SwaggerDoc("v1",...): Đặt tiêu đề và version của tài liệu Swagger.
    3. AddSecurityDefinition("Bearer",...): Định nghĩa xác dụng JWT Bearer Token.
    4. AddSecurityRequirement(...): Áp dụng xác dụng Bearer cho toàn bộ API(để 
        Swagger gửi token khi test)*/
});

//=> Kết quả trên Swagger UI
//Sau khi chạy app và mở Swagger(thường tại/swagger):
//- Bạn thấy tiêu đề "I Am Service v1"
//- Có nút "Authorize" ở góc trên phải.
//- Khi bạn nhập token JWT hợp lệ -> Swagger sẽ tự động gắn token đó vào header
//Authorization: Bearer <token> cho tất cả các request mà bạn test trong UI.

// -----------------------------
// Kestrel
// -----------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // REST API
});



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()    // hoặc thay bằng domain frontend cụ thể
              .AllowAnyHeader()    //vd: policy.WithOrigins("https://myfrontend.com");
              .AllowAnyMethod();
    });
});

//set up db
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<IAMDBContext>(options => options.UseSqlServer(connectionString!));


//JWT
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //Kích hoạt hệ thống xác thực trong ASP.NET Core, chọn loại là JWT Bearer
    .AddJwtBearer(options =>
    //cấu hình cách kiểm tra và xác minh token JWT
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

builder.Services.AddAuthorization();
//Bật cơ chế phân quyền(authorization) để xác định người dùng có được phép truy cập
//endpoint nào không?


//DI
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();


//DI for repositories and unit of work
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

var app = builder.Build();

//Auto migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IAMDBContext>();

    var retries = 5;
    for (int i = 0; i < retries; i++)
    {
        try
        {
            // Create DB if it doesnt exist and apply all migrations
            await db.Database.MigrateAsync();
            await Seeder.SeedAdminRoleAsync(db);
            Console.WriteLine("Database migrated and seeded successfully.");
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
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication(); // bắt buộc nếu dùng JWT
//app.UseCors();           // **phải trước UseAuthorization**
app.UseAuthorization();

app.MapControllers();

app.Run();
