using AppointmentManager.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Application.Interfaces;

public interface IAppointmentDbContext
{
    DbSet<User> Users { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<Appointment> Appointments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
