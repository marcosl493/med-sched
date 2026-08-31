using Application.Factorys;
using Application.Options;
using Domain.Entities;
using Domain.Events;
using Microsoft.Extensions.Options;

namespace ApplicationTests.Services;

[TestClass]
public class AppointmentNotificationFactoryTests
{
    [TestMethod]
    public void Create_ShouldBuildRequest_WithPtFormatting()
    {
        // Arrange
        var options = new AppointmentCreatedNotificationOptions
        {
            ApplicationCode = "app-code",
            Messages = new Dictionary<string, AppointmentCreatedNotificationOptions.Message>
            {
                ["pt-BR"] = new AppointmentCreatedNotificationOptions.Message
                {
                    Title = "Olá {UserName}",
                    Body = "Consulta em {AppointmentDate} às {AppointmentTime}"
                }
            }
        };

        var factory = new AppointmentNotificationFactory(Options.Create(options));

        var start = new DateTimeOffset(2026, 8, 31, 15, 30, 0, TimeSpan.Zero);
        var evt = new AppointmentCreatedEvent(Guid.NewGuid(), "reason", AppointmentStatus.SCHEDULED, Guid.NewGuid(), new UserDto(Guid.NewGuid(), "Maria", "m@x.com"), new ScheduleDto(Guid.NewGuid(), start, start.AddHours(1)), DateTimeOffset.UtcNow);

        var device = new Device(Guid.NewGuid(), "token-pt-1", "v1", "city", "BR", "model", "pt-BR", "android", true, "sdk", false, false, 0, Guid.NewGuid());
        var devices = new List<Device> { device };

        // Act
        var request = factory.Create(evt, devices);

        // Assert
        Assert.IsNotNull(request);
        Assert.IsNotNull(request.Transactional);
        Assert.IsNotNull(request.Transactional.Payload);
        Assert.IsNotNull(request.Transactional.Payload.Content);
        Assert.IsTrue(request.Transactional.Payload.Content.LocalizedContent.ContainsKey("pt-BR"));

        var content = request.Transactional.Payload.Content.LocalizedContent["pt-BR"];
        Assert.AreEqual("Olá Maria", content.Android.Title);
        Assert.AreEqual("Consulta em 31/08/2026 às 15:30", content.Android.Body);
        Assert.Contains("token-pt-1", [.. request.Transactional.PushTokens!.List!]);
    }

    [TestMethod]
    public void Create_ShouldBuildRequest_WithEnFormatting()
    {
        // Arrange
        var options = new AppointmentCreatedNotificationOptions
        {
            ApplicationCode = "app-code",
            Messages = new Dictionary<string, AppointmentCreatedNotificationOptions.Message>
            {
                ["en-US"] = new AppointmentCreatedNotificationOptions.Message
                {
                    Title = "Hello {UserName}",
                    Body = "Appointment on {AppointmentDate} at {AppointmentTime}"
                }
            }
        };

        var factory = new AppointmentNotificationFactory(Options.Create(options));

        var start = new DateTimeOffset(2026, 8, 31, 15, 30, 0, TimeSpan.Zero);
        var evt = new AppointmentCreatedEvent(Guid.NewGuid(), "reason", AppointmentStatus.SCHEDULED, Guid.NewGuid(), new UserDto(Guid.NewGuid(), "John", "j@x.com"), new ScheduleDto(Guid.NewGuid(), start, start.AddHours(1)), DateTimeOffset.UtcNow);

        var device = new Device(Guid.NewGuid(), "token-en-1", "v1", "city", "US", "model", "en-US", "ios", true, "sdk", false, false, 0, Guid.NewGuid());
        var devices = new List<Device> { device };

        // Act
        var request = factory.Create(evt, devices);

        // Assert
        Assert.IsNotNull(request);
        Assert.IsTrue(request.Transactional!.Payload.Content.LocalizedContent.ContainsKey("en-US"));

        var content = request.Transactional.Payload.Content.LocalizedContent["en-US"];
        Assert.AreEqual("Hello John", content.Ios.Title);
        Assert.AreEqual("Appointment on 08/31/2026 at 03:30 PM", content.Ios.Body);
        Assert.Contains("token-en-1", [.. request.Transactional.PushTokens!.List!]);
    }
}
