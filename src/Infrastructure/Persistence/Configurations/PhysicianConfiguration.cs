using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PhysicianConfiguration : IEntityTypeConfiguration<Physician>
{
    public void Configure(EntityTypeBuilder<Physician> builder)
    {
        builder.ToTable("Physicians");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Specialty).IsRequired().HasMaxLength(100);
        builder.Property(p => p.LicenseNumber).IsRequired().HasMaxLength(50);
        builder.Property(p => p.StateAbbreviation).IsRequired().HasMaxLength(2);
        builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.UserId).IsRequired();
        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<Physician>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.AvailableSlots);
        builder.HasIndex(p => p.UserId).IsUnique();
    }
}
