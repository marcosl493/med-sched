using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");
        builder.HasKey(a => a.Hwid);
        builder.Property(a => a.PushToken).IsRequired().IsUnicode().HasMaxLength(256);
        builder.Property(a => a.ApplicationVersion).IsRequired().IsUnicode().HasMaxLength(100);
        builder.Property(a => a.City).IsRequired().IsUnicode().HasMaxLength(100);
        builder.Property(a => a.Country).IsRequired().IsUnicode().HasMaxLength(100);
        builder.Property(a => a.DeviceModel).IsRequired().IsUnicode().HasMaxLength(100);
        builder.Property(a => a.Language).IsRequired().IsUnicode().HasMaxLength(50);
        builder.Property(a => a.Platform).IsRequired().IsUnicode().HasMaxLength(100);
        builder.Property(a => a.SdkVersion).IsRequired().IsUnicode().HasMaxLength(100);
        builder.Property(a => a.PushAlertsEnabled).IsRequired();
        builder.Property(a => a.ScheduledSummary).IsRequired();
        builder.Property(a => a.TimeSensitiveNotifications).IsRequired();
        builder.Property(a => a.Timezone).IsRequired();
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.ApplicationCode).IsRequired().IsUnicode().HasMaxLength(15);
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.HasOne(a => a.User)
            .WithMany(u => u.Devices)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.UserId).HasDatabaseName("IX_Devices_UserId");
    }

}
