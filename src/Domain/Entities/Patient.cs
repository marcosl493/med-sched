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
    public Patient(DateTime dateOfBirth, string name, string email, string password)
    {
        Id = Guid.CreateVersion7();
        DateOfBirth = dateOfBirth;
        User = new User(name, email, UserRole.PATIENT, password);
    }
    public Appointment ScheduleAppointment(Guid scheduleId, string reason)
    {
        var newAppointment = new Appointment(reason, scheduleId, Id);
        Appointments.Add(newAppointment);
        return newAppointment;
    }
}
