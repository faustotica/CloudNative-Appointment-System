using MediatR;

namespace AppointmentManager.Application.Commands;

public record CreateAppointmentCommand(int DoctorId, int PatientId, DateTime AppointmentDate) : IRequest<int>;
