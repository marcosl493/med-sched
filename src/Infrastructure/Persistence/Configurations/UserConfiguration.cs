﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(500);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Property(u => u.HashedPassword).IsRequired().HasMaxLength(60);
        builder.Property(u => u.Role).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
