namespace WAPI.Dtos;

public record PatientDto(int Id, string LastName, string FirstName, string? Patronymic, DateOnly BirthDate, string Address);
public record CreatePatientDto(string LastName, string FirstName, string? Patronymic, DateOnly BirthDate, string Address);
public record UpdatePatientDto(string LastName, string FirstName, string? Patronymic, DateOnly BirthDate, string Address);