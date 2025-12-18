namespace WAPI.Dtos;

public record DiagnosisDto(int Id, string Code, string Name);
public record CreateDiagnosisDto(string Code, string Name);
public record UpdateDiagnosisDto(string Code, string Name);