namespace Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public AppointmentStatus Status { get; private set; }
    public Guid PatientId { get; private set; }
    public virtual Patient Patient { get; private set; } = null!;
    public Guid ScheduleId { get; private set; }
    public virtual Schedule Schedule { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Appointment(string reason, AppointmentStatus status, Guid scheduleId, Guid patientId)
    {
        Id = Guid.CreateVersion7();
        Reason = reason;
        Status = status;
        PatientId = patientId;
        ScheduleId = scheduleId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public Appointment()
    {
        
    }
    public void UpdateStatus(AppointmentStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}
public enum AppointmentStatus
{
    SCHEDULED = 1,
    CONFIRMED,
    COMPLETED,
    CANCELED,
    RESCHEDULED,
    NOSHOW
};

