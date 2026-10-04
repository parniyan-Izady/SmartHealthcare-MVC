using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Infrastructure.Identity;

namespace SmartHealthcare.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.MedicalLicenseNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(d => d.MedicalLicenseNumber)
            .IsUnique();

        builder.Property(d => d.MedicalSpecialty)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(d => d.MedicalSpecialty);

        builder.Property(d => d.ConsultationFee)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(d => d.OfficeAddress)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Doctor>(d => d.IdentityUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Appointments)
            .WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
