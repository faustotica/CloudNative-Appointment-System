using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppointmentManager.Infrastructure.Data;
using AppointmentManager.Core.Entities;
using AppointmentManager.Application.DTOs;

namespace AppointmentManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppointmentDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppointmentDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
    {
        // Verificar si el usuario ya existe
        var userExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (userExists)
        {
            return BadRequest(new { message = "Ya existe un usuario registrado con este correo electrónico." });
        }

        // Crear el nuevo usuario
        var newUser = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = request.PasswordHash, // En un entorno de producción real esto iría hasheado con BCrypt, pero para empezar sirve perfecto
            Role = string.IsNullOrEmpty(request.Role) ? "Patient" : request.Role
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Usuario registrado exitosamente.", userId = newUser.Id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        // 1. Buscar el usuario en la base de datos por su email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || user.PasswordHash != request.PasswordHash)
        {
            return Unauthorized(new { message = "Email o contraseña incorrectos." });
        }

        // 2. Definir los "Claims" (información interna que viaja dentro del token firmada de forma segura)
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        // 3. Obtener la clave secreta desde appsettings.json
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException());

        // 4. Configurar el descriptor del token (duración, emisor, algoritmo de firma)
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(3), // El token expira en 3 horas
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        // 5. Crear y escribir el token en formato string
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        // 6. Devolver el token al cliente
        return Ok(new
        {
            Token = jwtToken,
            Email = user.Email,
            Role = user.Role
        });
    }
}