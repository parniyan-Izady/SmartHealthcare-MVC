namespace SmartHealthcare.Application.DTOs;

public record CreateDoctorRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string MedicalLicenseNumber,
    string MedicalSpecialty,
    decimal ConsultationFee,
    string OfficeAddress
);

public record UpdateDoctorRequest(
    string FirstName,
    string LastName,
    string MedicalSpecialty,
    decimal ConsultationFee,
    string OfficeAddress
);

public record DoctorResponse(
    Guid Id,
    Guid UserId,
    string FullName,
    string Email,
    string MedicalLicenseNumber,
    string MedicalSpecialty,
    decimal ConsultationFee,
    string OfficeAddress,
    bool IsActive
);

public record DoctorFilterRequest(
    string? Specialty = null,
    string? SearchTerm = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = "LastName",
    string? SortOrder = "asc"
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
