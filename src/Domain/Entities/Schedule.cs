namespace Domain.Entities;

public class Schedule
{
    public Schedule(Guid physicianId, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        Id = Guid.CreateVersion7();
        PhysicianId = physicianId;
        CreatedAt = DateTime.UtcNow;
        StartTime = startTime;
        EndTime = endTime;
    }
    public Schedule(Guid id, ICollection<Appointment> appointments, Physician physician, DateTimeOffset createdAt, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        Physician = physician;
        CreatedAt = createdAt;
        StartTime = startTime;
        EndTime = endTime;
        Id = id;
        Appointments = appointments;
        PhysicianId = physician.Id;
    }
    public void Update(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
    public Schedule()
    {

    }
    public bool IsAvaliableSchedule()
         => !Appointments.Any(appointment => appointment.Status == AppointmentStatus.SCHEDULED);
    public Guid Id { get; private set; }
    public Guid PhysicianId { get; private set; }
    public virtual Physician Physician { get; private set; } = null!;
    public virtual ICollection<Appointment> Appointments { get; private set; } = [];
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}
