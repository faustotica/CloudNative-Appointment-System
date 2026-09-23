namespace AppointmentManager.Application.DTOs;

public class CreateDoctorRequest
{
    public int UserId { get; set; }
    public string Specialty { get; set; } = string.Empty;
}