using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Reason).IsRequired().HasMaxLength(500);
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired();
        builder.Property(a => a.PatientId).IsRequired();
        builder.Property(a => a.ScheduleId).IsRequired();
        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.Schedule)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(a => new { a.PatientId, a.ScheduleId, a.Status }).IsUnique();
    }
}
