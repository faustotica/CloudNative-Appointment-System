using System;
using System.Threading.Tasks;
using AppointmentManager.Core.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AppointmentManager.Worker.Consumers;

public class AppointmentCreatedConsumer : IConsumer<AppointmentCreatedEvent>
{
    private readonly ILogger<AppointmentCreatedConsumer> _logger;

    public AppointmentCreatedConsumer(ILogger<AppointmentCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<AppointmentCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("==================================================");
        _logger.LogInformation($"[Worker] Evento recibido: Turno Creado (ID: {message.AppointmentId})");
        _logger.LogInformation($"[Worker] Simulando envío de email para paciente {message.PatientId} y médico {message.DoctorId}");
        _logger.LogInformation($"[Worker] Fecha: {message.AppointmentDate}");
        _logger.LogInformation("==================================================");

        // Simulando delay de envío de correo
        return Task.Delay(1000);
    }
}
