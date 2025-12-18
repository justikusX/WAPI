namespace WAPI.Dtos;

public record LoginRequest(string Login, string Password);
public record LoginResponse(string AccessToken);