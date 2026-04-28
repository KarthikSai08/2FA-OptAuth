using Api.Middleware;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

builder.Services.AddOpenApi();
// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddTransient<IDbConnection>(_ =>
    new SqlConnection(cfg.GetConnectionString("Default")));

// ── Repositories ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();

// ── SMTP / MailKit ────────────────────────────────────────────────────────────
var smtpSettings = cfg.GetSection("Smtp").Get<SmtpSettings>()!;
builder.Services.AddSingleton(smtpSettings);
builder.Services.AddScoped<IEmailService, EmailService>();

// ── JWT ───────────────────────────────────────────────────────────────────────
var jwtSettings = cfg.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

// ── Auth Service ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();

// ── Infrastructure ────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<DapperContext>();


//builder.Services.AddSwaggerGen(o =>
//{
//    o.SwaggerDoc("v1", new() { Title = "OTP Auth API", Version = "v1" });
//    o.AddSecurityDefinition("Bearer", new()
//    {
//        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Description = "Enter: Bearer {token}",
//        Name = "Authorization",
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });
//    o.AddSecurityRequirement(new()
//    {
//        {
//            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
//            Array.Empty<string>()
//        }
//    });
//});

// ── App ───────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
