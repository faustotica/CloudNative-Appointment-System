using MediatR;

namespace AppointmentManager.Application.Commands;

public record CancelAppointmentCommand(int Id) : IRequest<bool>;
