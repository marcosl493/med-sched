namespace Domain.Entities;

public class Physician
{
    public Guid Id { get; private set; }
    public string Specialty { get; private set; } = string.Empty;
    public string LicenseNumber { get; private set; } = string.Empty;
    public string StateAbbreviation { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public virtual ICollection<Schedule> AvailableSlots { get; private set; } = [];
    public Physician(string specialty, string licenseNumber, string stateAbbreviation, Guid userId)
    {
        Id = Guid.CreateVersion7();
        Specialty = specialty;
        LicenseNumber = licenseNumber;
        StateAbbreviation = stateAbbreviation;
        UserId = userId;
        IsActive = true;
    }
    public Physician()
    {
        
    }
    public void AddAvailableSlots(List<Schedule> slotsToAdd)
    {
        var combinedSlots = new List<Schedule>();
        combinedSlots.AddRange(AvailableSlots);
        combinedSlots.AddRange(slotsToAdd);

        if (HasOverlappingSlots(combinedSlots))
        {
            throw new ArgumentException("New slots overlap with existing available slots.");
        }
        foreach (var item in slotsToAdd)
        {
            AvailableSlots.Add(item);
        }
        
    }
    private static bool HasOverlappingSlots(List<Schedule> slots)
    {
        if (slots == null || slots.Count <= 1)
        {
            return false;
        }

        var sortedSlots = slots.OrderBy(s => s.StartTime).ToList();

        for (int i = 0; i < sortedSlots.Count - 1; i++)
        {
            if (sortedSlots[i + 1].StartTime < sortedSlots[i].EndTime)
            {
                return true;
            }
        }

        return false;
    }
}
