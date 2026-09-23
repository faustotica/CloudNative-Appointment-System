using AppointmentManager.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentManager.Application.Queries;

public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IAppointmentDbContext _context;

    public GetAppointmentsQueryHandler(IAppointmentDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Appointments
            .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
            .Select(a => new AppointmentDto(
                a.Id,
                a.AppointmentDate,
                a.Status,
                a.DoctorId,
                a.Doctor.User.Name,
                a.Doctor.Specialty,
                a.PatientId
            ))
            .ToListAsync(cancellationToken);
    }
}
