namespace WAPI.Dtos;

public record VisitDto(int Id, int DoctorId, int PatientId, int DiagnosisId, DateOnly VisitDate);

public record VisitDetailsDto(
    int Id,
    DateOnly VisitDate,
    int DoctorId, string DoctorFullName, string DoctorSpecialty,
    int PatientId, string PatientFullName,
    int DiagnosisId, string DiagnosisCode, string DiagnosisName
);

public record CreateVisitDto(int DoctorId, int PatientId, int DiagnosisId, DateOnly VisitDate);
public record UpdateVisitDto(int DoctorId, int PatientId, int DiagnosisId, DateOnly VisitDate);