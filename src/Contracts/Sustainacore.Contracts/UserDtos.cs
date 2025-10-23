namespace Sustainacore.Contracts;

public record UserDto(
    string Uid,
    string Email,
    string DisplayName,
    string? Role,
    bool EmailVerified
);

public record UpdateRoleRequest(string Role);
