using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentManager.Infrastructure.Data;
using AppointmentManager.Core.Entities;
using AppointmentManager.Application.DTOs;

namespace AppointmentManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorsController : ControllerBase
{
    private readonly AppointmentDbContext _context;

    public DoctorsController(AppointmentDbContext context)
    {
        _context = context;
    }

    // GET: api/doctors (Público o protegido, para que cualquiera pueda listar los médicos disponibles)
    [HttpGet]
    public async Task<IActionResult> GetDoctors()
    {
        var doctors = await _context.Doctors
            .Include(d => d.User) // Trae los datos del usuario asociado (Nombre, Email)
            .Select(d => new
            {
                d.Id,
                d.Specialty,
                DoctorName = d.User.Name,
                Email = d.User.Email
            })
            .ToListAsync();

        return Ok(doctors);
    }

    // POST: api/doctors (Protegido: Solo accesible si envías el token JWT en Swagger)
    [HttpPost]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorRequest request)
    {
        // Verificar que el usuario exista
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            return NotFound(new { message = "El usuario especificado no existe." });
        }

        // Verificar si el usuario ya es doctor
        var existingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == request.UserId);
        if (existingDoctor != null)
        {
            return BadRequest(new { message = "Este usuario ya está registrado como médico." });
        }

        var newDoctor = new Doctor
        {
            UserId = request.UserId,
            Specialty = request.Specialty
        };

        _context.Doctors.Add(newDoctor);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Médico registrado exitosamente.", doctorId = newDoctor.Id });
    }
}