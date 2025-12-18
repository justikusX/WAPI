namespace WAPI.Dtos;

public record DoctorDto(int Id, string LastName, string FirstName, string? Patronymic, string Specialty, int Experience);
public record CreateDoctorDto(string LastName, string FirstName, string? Patronymic, string Specialty, int Experience);
public record UpdateDoctorDto(string LastName, string FirstName, string? Patronymic, string Specialty, int Experience);