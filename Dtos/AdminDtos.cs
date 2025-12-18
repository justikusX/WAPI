namespace WAPI.Dtos;

public record AdminDto(
    int Id,
    string Login,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    DateTime? LastLogin,
    bool IsActive,
    string Role
);

public record CreateAdminDto(
    string Login,
    string Password,
    string FirstName,
    string LastName,
    bool IsActive,
    string Role
);

public record UpdateAdminDto(
    string FirstName,
    string LastName,
    bool IsActive,
    string Role
);

public record ChangePasswordDto(string NewPassword);