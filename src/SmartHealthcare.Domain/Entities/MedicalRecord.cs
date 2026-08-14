using SmartHealthcare.Domain.Common;

namespace SmartHealthcare.Domain.Entities;

public class MedicalRecord : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = default!;

    public Guid DoctorId { get; private set; }
    public Doctor Doctor { get; private set; } = default!;

    public string Diagnosis { get; private set; } = default!;
    public string PrescriptionNotes { get; private set; } = default!;
    public string? TreatmentPlan { get; private set; }

    private MedicalRecord() { }

    public MedicalRecord(Guid patientId, Guid doctorId, string diagnosis, string prescriptionNotes, string? treatmentPlan)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        Diagnosis = diagnosis;
        PrescriptionNotes = prescriptionNotes;
        TreatmentPlan = treatmentPlan;
    }
}
