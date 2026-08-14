using System.ComponentModel.DataAnnotations;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Presentation.ViewModels.Doctor;

public class DoctorListViewModel
{
    public IReadOnlyList<DoctorResponse> Doctors { get; set; } = Array.Empty<DoctorResponse>();
    public string? Specialty { get; set; }
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
    public string? SortBy { get; set; } = "LastName";
    public string? SortOrder { get; set; } = "asc";
}

public class DoctorCreateViewModel
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Medical License Number is required.")]
    [StringLength(50)]
    [Display(Name = "Medical License Number")]
    public string MedicalLicenseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Medical Specialty is required.")]
    [StringLength(100)]
    [Display(Name = "Medical Specialty")]
    public string MedicalSpecialty { get; set; } = string.Empty;

    [Required(ErrorMessage = "Consultation fee is required.")]
    [Range(0, 1000000, ErrorMessage = "Consultation fee must be a positive number.")]
    [Display(Name = "Consultation Fee ($)")]
    public decimal ConsultationFee { get; set; }

    [Required(ErrorMessage = "Office address is required.")]
    [StringLength(250)]
    [Display(Name = "Office Address")]
    public string OfficeAddress { get; set; } = string.Empty;
}

public class DoctorEditViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Email Address")]
    public string? Email { get; set; }

    [Display(Name = "Medical License Number")]
    public string? MedicalLicenseNumber { get; set; }

    [Required(ErrorMessage = "Medical Specialty is required.")]
    [StringLength(100)]
    [Display(Name = "Medical Specialty")]
    public string MedicalSpecialty { get; set; } = string.Empty;

    [Required(ErrorMessage = "Consultation fee is required.")]
    [Range(0, 1000000, ErrorMessage = "Consultation fee must be a positive number.")]
    [Display(Name = "Consultation Fee ($)")]
    public decimal ConsultationFee { get; set; }

    [Required(ErrorMessage = "Office address is required.")]
    [StringLength(250)]
    [Display(Name = "Office Address")]
    public string OfficeAddress { get; set; } = string.Empty;
}

public class DoctorDetailsViewModel
{
    public DoctorResponse Doctor { get; set; } = null!;
}
