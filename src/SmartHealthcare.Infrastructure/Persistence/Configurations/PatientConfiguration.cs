using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Infrastructure.Identity;

namespace SmartHealthcare.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(150);
        builder.Property(p => p.NationalCode).IsRequired().HasMaxLength(10);
        builder.HasIndex(p => p.NationalCode).IsUnique();
        builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Gender).HasConversion<string>().HasMaxLength(10);

        builder.HasOne<ApplicationUser>()
               .WithOne()
               .HasForeignKey<Patient>(p => p.IdentityUserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
