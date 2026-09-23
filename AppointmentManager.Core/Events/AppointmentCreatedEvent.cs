using System;

namespace AppointmentManager.Core.Events;

public record AppointmentCreatedEvent
{
    public int AppointmentId { get; init; }
    public int PatientId { get; init; }
    public int DoctorId { get; init; }
    public DateTime AppointmentDate { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
