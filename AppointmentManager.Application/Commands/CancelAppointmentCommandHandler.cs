using AppointmentManager.Application.Interfaces;
using MediatR;

namespace AppointmentManager.Application.Commands;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, bool>
{
    private readonly IAppointmentDbContext _context;

    public CancelAppointmentCommandHandler(IAppointmentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { request.Id }, cancellationToken);
        if (appointment == null)
            throw new Exception("El turno especificado no existe.");
            
        if (appointment.Status == "Cancelado")
            throw new Exception("El turno ya se encuentra cancelado.");

        appointment.Status = "Cancelado";
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
