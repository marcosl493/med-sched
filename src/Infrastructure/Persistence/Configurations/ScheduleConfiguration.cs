using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("Schedules");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.PhysicianId).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.StartTime).IsRequired();
        builder.Property(s => s.EndTime).IsRequired();
        builder.Property(s => s.PhysicianId).IsRequired();
        builder.HasOne(s => s.Physician)
            .WithMany(p => p.AvailableSlots)
            .HasForeignKey(s => s.PhysicianId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.Appointments)
            .WithOne(a => a.Schedule)
            .HasForeignKey(a => a.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.PhysicianId, s.StartTime, s.EndTime }).IsUnique();
    }
}
