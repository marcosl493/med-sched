namespace Domain.Entities;

public class Schedule
{
    public Schedule(Guid physicianId, DateTime createdAt, DateTime startTime, DateTime endTime)
    {
        Id = Guid.CreateVersion7();
        PhysicianId = physicianId;
        CreatedAt = createdAt;
        StartTime = startTime;
        EndTime = endTime;
    }
    public Schedule()
    {

    }
    public Guid Id { get; private set; }
    public Guid PhysicianId { get; private set; }
    public Physician Physician { get; private set; } = null!;
    public List<Appointment> Appointments { get; private set; } = [];
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
