using System.Security.Claims;

namespace LeaveManagementSystem.Application.Interfaces;

public interface IJwtService
{
    (string accessToken, DateTime expiresAt) GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
