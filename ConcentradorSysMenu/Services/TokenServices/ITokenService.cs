using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ConcentradorSysMenu.Services.TokenServices;

public interface ITokenService
{
    JwtSecurityToken GeraToken(IEnumerable<Claim> claims, IConfiguration _config);

    string? GeraRefreshToken();

    ClaimsPrincipal GetClaimsPrincipal(string Token, IConfiguration _config);
}
