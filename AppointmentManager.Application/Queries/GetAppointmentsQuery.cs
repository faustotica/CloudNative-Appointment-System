using MediatR;
using System.Collections.Generic;

namespace AppointmentManager.Application.Queries;

public record AppointmentDto(int Id, DateTime AppointmentDate, string Status, int DoctorId, string DoctorName, string Specialty, int PatientId);

public record GetAppointmentsQuery() : IRequest<List<AppointmentDto>>;
