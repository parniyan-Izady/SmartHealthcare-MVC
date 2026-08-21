namespace SmartHealthcare.Application.DTOs;

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
