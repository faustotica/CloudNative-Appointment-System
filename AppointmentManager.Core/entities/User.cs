using Microsoft.AspNetCore.Identity;

namespace AppointmentManager.Core.Entities;

public class User : IdentityUser<int>
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "Patient"; // "Admin", "Doctor", "Patient"

    public ICollection<Appointment> AppointmentsAsPatient { get; set; } = new List<Appointment>();
}