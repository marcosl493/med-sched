namespace Domain.Entities;

public class Schedule
{
    public Schedule(Guid physicianId, DateTime startTime, DateTime endTime)
    {
        Id = Guid.CreateVersion7();
        PhysicianId = physicianId;
        CreatedAt = DateTime.UtcNow;
        StartTime = startTime;
        EndTime = endTime;
    }
    public Schedule(Guid id, Physician physician, DateTime createdAt, DateTime startTime, DateTime endTime)
    {
        Physician = physician;
        CreatedAt = createdAt;
        StartTime = startTime;
        EndTime = endTime;
        Id = id;
    }
    public void Update(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
    public Schedule()
    {

    }
    public Guid Id { get; private set; }
    public Guid PhysicianId { get; private set; }
    public virtual Physician Physician { get; private set; } = null!;
    public List<Appointment> Appointments { get; private set; } = [];
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
