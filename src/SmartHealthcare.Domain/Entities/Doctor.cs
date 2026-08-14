using SmartHealthcare.Domain.Common;

namespace SmartHealthcare.Domain.Entities;

public class Doctor : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public string MedicalLicenseNumber { get; private set; } = default!;
    public string MedicalSpecialty { get; private set; } = default!;
    public decimal ConsultationFee { get; private set; }
    public string OfficeAddress { get; private set; } = default!;

    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

    private Doctor() { }

    public Doctor(Guid userId, string licenseNumber, string specialty, decimal consultationFee, string officeAddress)
    {
        UserId = userId;
        MedicalLicenseNumber = licenseNumber;
        MedicalSpecialty = specialty;
        ConsultationFee = consultationFee;
        OfficeAddress = officeAddress;
    }
}
