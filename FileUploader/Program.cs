using System.IdentityModel.Tokens.Jwt;
using FileUploader.Config;
using FileUploader.Data;
using FileUploader.Services;
using FileUploader.Services.PasswordHasher;
using FileUploader.Services.Tokens;
using FileUploader.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.Load();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<Argon2Settings>(builder.Configuration.GetSection("Argon2"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Missing Jwt config in .env");

var argon2Settings = builder.Configuration.GetSection("Argon2").Get<Argon2Settings>()
    ?? throw new InvalidOperationException("Missing Argon2 config in .env");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)
    );

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtConfig.GetValidationParameters(jwtSettings);
        options.Events = JwtConfig.GetJwtBearerEvents();
    });
    
builder.Services.AddControllers();
builder.Services.AddOpenApi();
    
// scoped - one instance per request
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<TokenGenerator>();

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton(argon2Settings);

builder.Services.AddSingleton<IPasswordHasher, IsopohPasswordHasher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/openapi/v1.json", "FileUploader API v1");
    });
}
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();