using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppointmentManager.Application.Commands;
using AppointmentManager.Application.Interfaces;
using AppointmentManager.Core.Entities;
using AppointmentManager.Core.Events;
using MockQueryable.Moq;
using Moq;
using MassTransit;
using Xunit;

namespace AppointmentManager.Tests;

public class CreateAppointmentCommandHandlerTests
{
    private readonly Mock<IAppointmentDbContext> _mockContext;
    private readonly Mock<IPublishEndpoint> _mockPublisher;
    private readonly CreateAppointmentCommandHandler _handler;

    public CreateAppointmentCommandHandlerTests()
    {
        _mockContext = new Mock<IAppointmentDbContext>();
        _mockPublisher = new Mock<IPublishEndpoint>();
        _handler = new CreateAppointmentCommandHandler(_mockContext.Object, _mockPublisher.Object);
    }

    [Fact]
    public async Task Handle_GivenValidRequest_ShouldCreateAppointmentAndReturnId()
    {
        // Arrange
        var doctorId = 1;
        var patientId = 2;
        var appointmentDate = DateTime.Now.AddDays(1);

        var doctor = new Doctor { Id = doctorId };
        
        _mockContext.Setup(c => c.Doctors.FindAsync(new object[] { doctorId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        var appointments = new List<Appointment>().BuildMockDbSet();
        _mockContext.Setup(c => c.Appointments).Returns(appointments.Object);

        var command = new CreateAppointmentCommand(doctorId, patientId, appointmentDate);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Appointments.Add(It.IsAny<Appointment>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockPublisher.Verify(p => p.Publish(It.IsAny<AppointmentCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenDoctorDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var doctorId = 1;
        var command = new CreateAppointmentCommand(doctorId, 2, DateTime.Now);

        Doctor? doctor = null;
        _mockContext.Setup(c => c.Doctors.FindAsync(new object[] { doctorId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("El médico especificado no existe.", exception.Message);
    }

    [Fact]
    public async Task Handle_GivenDoctorIsBusy_ShouldThrowException()
    {
        // Arrange
        var doctorId = 1;
        var appointmentDate = DateTime.Now.AddDays(1);
        var doctor = new Doctor { Id = doctorId };

        _mockContext.Setup(c => c.Doctors.FindAsync(new object[] { doctorId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctor);

        var existingAppointment = new Appointment { DoctorId = doctorId, AppointmentDate = appointmentDate };
        var appointments = new List<Appointment> { existingAppointment }.BuildMockDbSet();
        
        _mockContext.Setup(c => c.Appointments).Returns(appointments.Object);

        var command = new CreateAppointmentCommand(doctorId, 2, appointmentDate);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("El médico ya tiene un turno asignado en ese horario.", exception.Message);
    }
}
