using SmartHealthcare.Domain.Common;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Domain.Entities;

public class Doctor : BaseEntity
{
    public Guid IdentityUserId { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public string MedicalLicenseNumber { get; private set; } = default!;
    public string MedicalSpecialty { get; private set; } = default!;
    public decimal ConsultationFee { get; private set; }
    public string OfficeAddress { get; private set; } = default!;

    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

    private Doctor() { }

    public Doctor(
        Guid identityUserId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string licenseNumber,
        string specialty,
        decimal consultationFee,
        string officeAddress)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
        {
            throw new InvalidMedicalLicenseNumberException();
        }

        if (consultationFee < 0)
        {
            throw new InvalidConsultationFeeException(consultationFee);
        }

        IdentityUserId = identityUserId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        MedicalLicenseNumber = licenseNumber;
        MedicalSpecialty = specialty;
        ConsultationFee = consultationFee;
        OfficeAddress = officeAddress;
    }

    public void UpdateProfile(string firstName, string lastName, string email, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        MarkUpdated();
    }

    public void UpdateDetails(string specialty, decimal consultationFee, string officeAddress)
    {
        if (consultationFee < 0)
        {
            throw new InvalidConsultationFeeException(consultationFee);
        }

        MedicalSpecialty = specialty;
        ConsultationFee = consultationFee;
        OfficeAddress = officeAddress;
        MarkUpdated();
    }
}
