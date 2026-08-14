using SmartHealthcare.Domain.Common;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Domain.Entities;

public class Patient : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public string NationalCode { get; private set; } = default!;
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string PhoneNumber { get; private set; } = default!;
    public string? MedicalInsuranceNumber { get; private set; }
    public string? BloodGroup { get; private set; }

    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; private set; } = new List<MedicalRecord>();

    private Patient() { }

    public Patient(Guid userId, string nationalCode, DateTime dateOfBirth, Gender gender, string phoneNumber, string? insuranceNumber = null, string? bloodGroup = null)
    {
        UserId = userId;
        NationalCode = nationalCode;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        PhoneNumber = phoneNumber;
        MedicalInsuranceNumber = insuranceNumber;
        BloodGroup = bloodGroup;
    }

    public void UpdateContactInfo(string phoneNumber, string? insuranceNumber)
    {
        PhoneNumber = phoneNumber;
        MedicalInsuranceNumber = insuranceNumber;
        MarkUpdated();
    }
}
