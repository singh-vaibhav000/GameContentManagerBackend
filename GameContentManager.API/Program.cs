using GameContentManager.API.Data;
using GameContentManager.API.Models;
using GameContentManager.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbContext>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"].PadRight(32, '0').Substring(0, 32));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameCharacterService, GameCharacterService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serialize enums as strings in API responses
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// BEFORE app.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.WebHost.UseUrls("http://*:10000");
var app = builder.Build();
builder.Services.AddCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Use CORS policy
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create initial users if they don't exist
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var mongoContext = services.GetRequiredService<MongoDbContext>();
    
    // Create admin user
    var adminUser = new User
    {
        Email = "admin@example.com",
        PasswordHash = "admin123",
        Roles = new List<UserRole> { UserRole.Admin },
        MustChangePassword = false
    };

    // Create writer user
    var writerUser = new User
    {
        Email = "writer@example.com",
        PasswordHash = "writer123",
        Roles = new List<UserRole> { UserRole.Writer },
        MustChangePassword = false
    };

    // Create reader user
    var readerUser = new User
    {
        Email = "reader@example.com",
        PasswordHash = "reader123",
        Roles = new List<UserRole> { UserRole.Reader },
        MustChangePassword = false
    };

    // Check if users exist and insert if they don't
    var filter = Builders<User>.Filter;
    if (!await mongoContext.Users.Find(filter.Eq(u => u.Email, adminUser.Email)).AnyAsync())
    {
        await mongoContext.Users.InsertOneAsync(adminUser);
    }
    if (!await mongoContext.Users.Find(filter.Eq(u => u.Email, writerUser.Email)).AnyAsync())
    {
        await mongoContext.Users.InsertOneAsync(writerUser);
    }
    if (!await mongoContext.Users.Find(filter.Eq(u => u.Email, readerUser.Email)).AnyAsync())
    {
        await mongoContext.Users.InsertOneAsync(readerUser);
    }
}

//app.UseCors("CorsPolicy");

app.Run();


