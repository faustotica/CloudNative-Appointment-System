using AppointmentManager.Application.Interfaces;
using AppointmentManager.Core.Entities;
using Microsoft.AspNetCore.Identity;
using MassTransit;
using AppointmentManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddOpenApi();

// --- CONFIGURACIÓN DE BASE DE DATOS E IDENTITY ---

builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseInMemoryDatabase("TurneroLocalDb"));

builder.Services.AddScoped<IAppointmentDbContext>(provider => provider.GetRequiredService<AppointmentDbContext>());

builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppointmentDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IAppointmentDbContext).Assembly));

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CONFIGURACIÓN DE AUTENTICACIÓN JWT ---
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret key is not configured.");

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

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// --- EJECUTAR MIGRACIONES AUTOMÁTICAMENTE AL INICIAR ---
// (Ideal para proyectos de portfolio en Docker)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();
    // Asegura que la base de datos esté creada (En memoria no soporta Migrate, usamos EnsureCreated)
    db.Database.EnsureCreated();
}

app.Run();