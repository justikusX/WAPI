namespace WAPI.Dtos;

public record RegisterRequest(
    string Login,
    string Password,
    string FirstName,
    string LastName,
    string? Role
);
