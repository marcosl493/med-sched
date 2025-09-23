using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class MedSchedDbContext(DbContextOptions<MedSchedDbContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Schedule> Schedules { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Physician> Physicians { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MedSchedDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
