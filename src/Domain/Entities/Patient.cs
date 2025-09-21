namespace Domain.Entities;

public class Patient
{
    public Guid Id { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public virtual User User { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public virtual ICollection<Appointment> Appointments { get; private set; } = [];
    public Patient()
    {
        
    }
    public Patient(DateTime dateOfBirth, Guid userId)
    {
        Id = Guid.CreateVersion7();
        DateOfBirth = dateOfBirth;
        UserId = userId;
    }
    public ICollection<Appointment> ScheduleAppointment(Guid scheduleId, string reason)
    {
        Appointments.Add(new Appointment(reason, AppointmentStatus.SCHEDULED, scheduleId, Id));
        return Appointments;
    }
}
