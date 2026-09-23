using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AppointmentManager.Application.Commands;
using AppointmentManager.Application.Queries;

namespace AppointmentManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointments()
    {
        var appointments = await _mediator.Send(new GetAppointmentsQuery());
        return Ok(appointments);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command)
    {
        var appointmentId = await _mediator.Send(command);
        return Ok(new { message = "¡Turno reservado con éxito!", appointmentId });
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        await _mediator.Send(new CancelAppointmentCommand(id));
        return Ok(new { message = "¡Turno cancelado con éxito!", appointmentId = id });
    }
}