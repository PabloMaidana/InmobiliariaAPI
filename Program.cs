using Inmobiliaria.Api.Data;
using Inmobiliaria.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Servicios
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<JwtService>();

// Options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Auth JWT
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        o.TokenValidationParameters = new() {
            ValidateIssuer = true, ValidateAudience = true, ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"], ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// Controllers + JSON
builder.Services.AddControllers().AddNewtonsoftJson();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Archivos estáticos
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads"));
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed inicial
DbSeeder.Seed(app);

app.Run();