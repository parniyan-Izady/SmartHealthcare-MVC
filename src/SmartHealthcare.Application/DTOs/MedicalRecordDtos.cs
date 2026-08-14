namespace SmartHealthcare.Application.DTOs;

public record CreateMedicalRecordRequest(
    Guid PatientId,
    Guid DoctorId,
    string Diagnosis,
    string PrescriptionNotes,
    string? TreatmentPlan
);

public record UpdateMedicalRecordRequest(
    string Diagnosis,
    string PrescriptionNotes,
    string? TreatmentPlan
);

public record MedicalRecordResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    Guid DoctorId,
    string DoctorName,
    string Diagnosis,
    string PrescriptionNotes,
    string? TreatmentPlan,
    DateTime CreatedAtUtc
);
