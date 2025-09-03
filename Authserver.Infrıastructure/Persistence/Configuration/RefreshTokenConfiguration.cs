﻿using AuthServerDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authserver.Infrıastructure.Persistence.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>

    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.ExpirationDate)
                .IsRequired();
            
            builder.Property(x => x.UserId)
                .IsRequired();
            
            builder.HasOne(x => x.AppUser)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasIndex(x => x.Token)
                .IsUnique();
            
            builder.HasIndex(x => x.UserId);
        }
    }
}
