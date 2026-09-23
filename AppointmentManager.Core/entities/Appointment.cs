namespace AppointmentManager.Core.Entities;

public class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int PatientId { get; set; }
    public User Patient { get; set; } = null!;

    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "Pending"; // "Pending", "Confirmed", "Cancelled"
}