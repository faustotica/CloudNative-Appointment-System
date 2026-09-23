using AppointmentManager.Application.Interfaces;
using AppointmentManager.Core.Entities;
using AppointmentManager.Core.Events;
using MediatR;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentManager.Application.Commands;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, int>
{
    private readonly IAppointmentDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateAppointmentCommandHandler(IAppointmentDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<int> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _context.Doctors.FindAsync(new object[] { request.DoctorId }, cancellationToken);
        if (doctor == null)
            throw new Exception("El médico especificado no existe.");

        var conflictingAppointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.DoctorId == request.DoctorId && a.AppointmentDate == request.AppointmentDate, cancellationToken);

        if (conflictingAppointment != null)
            throw new Exception("El médico ya tiene un turno asignado en ese horario.");

        var newAppointment = new Appointment
        {
            DoctorId = request.DoctorId,
            PatientId = request.PatientId,
            AppointmentDate = request.AppointmentDate,
            Status = "Confirmado"
        };

        _context.Appointments.Add(newAppointment);
        await _context.SaveChangesAsync(cancellationToken);

        // Publicar evento al Message Broker
        await _publishEndpoint.Publish(new AppointmentCreatedEvent
        {
            AppointmentId = newAppointment.Id,
            DoctorId = newAppointment.DoctorId,
            PatientId = newAppointment.PatientId,
            AppointmentDate = newAppointment.AppointmentDate
        }, cancellationToken);

        return newAppointment.Id;
    }
}
