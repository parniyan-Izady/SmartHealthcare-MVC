using SmartHealthcare.Domain.Common;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Domain.Entities;

public class Patient : BaseEntity
{
    public Guid IdentityUserId { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public string NationalCode { get; private set; } = default!;
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string? MedicalInsuranceNumber { get; private set; }
    public string? BloodGroup { get; private set; }

    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

    private Patient() { }

    public Patient(
        Guid identityUserId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string nationalCode,
        DateTime dateOfBirth,
        Gender gender,
        string? insuranceNumber = null,
        string? bloodGroup = null)
    {
        if (string.IsNullOrWhiteSpace(nationalCode) || nationalCode.Trim().Length != 10)
        {
            throw new InvalidNationalCodeException(nationalCode);
        }

        if (dateOfBirth > DateTime.UtcNow)
        {
            throw new InvalidDateOfBirthException(dateOfBirth);
        }

        IdentityUserId = identityUserId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        NationalCode = nationalCode;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        MedicalInsuranceNumber = insuranceNumber;
        BloodGroup = bloodGroup;
    }

    public void UpdateProfile(string firstName, string lastName, string email, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        MarkUpdated();
    }

    public void UpdateMedicalInfo(string? insuranceNumber, string? bloodGroup)
    {
        MedicalInsuranceNumber = insuranceNumber;
        BloodGroup = bloodGroup;
        MarkUpdated();
    }
}
