using Application.Integration.Pushwoosh;
using Domain.Entities;
using Domain.Events;

namespace Application.Interfaces.Services;

public interface IAppointmentNotificationFactory
{
    PushwooshNotifyRequest Create(AppointmentCreatedEvent message, IEnumerable<Device> devices);
}
