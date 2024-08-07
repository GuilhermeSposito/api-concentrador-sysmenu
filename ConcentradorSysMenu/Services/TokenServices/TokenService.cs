using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ConcentradorSysMenu.Services.TokenServices;
public class TokenService : ITokenService
{
    public JwtSecurityToken GeraToken(IEnumerable<Claim> claims, IConfiguration _config)
    {
        string? SecretKey = _config.GetRequiredSection("JWT").GetValue<string>("SecretKey") ?? throw new ArgumentException("Chave secreta inválida!");
        byte[] secretKey = Encoding.UTF8.GetBytes(SecretKey);

        SigningCredentials stringCredencial = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

        var TokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_config.GetRequiredSection("JWT").GetValue<int>("TokenValidityInMinutes")),
            Audience = _config.GetRequiredSection("JWT").GetValue<string>("ValidAudience"),
            Issuer = _config.GetRequiredSection("JWT").GetValue<string>("ValidIssuer"),
            SigningCredentials = stringCredencial
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(TokenDescriptor);

        return token;
    }
    public string? GeraRefreshToken()
    {
        var secureRandomBytes = new byte[128];

        using var randomNumberGenerator = RandomNumberGenerator.Create();

        randomNumberGenerator.GetBytes(secureRandomBytes);

        var refreshToken = Convert.ToBase64String(secureRandomBytes);

        return refreshToken;
    }

    public ClaimsPrincipal GetClaimsPrincipal(string Token, IConfiguration _config)
    {
        var secretKey = _config["JWT:SecretKey"] ?? throw new InvalidOperationException("Chave Invalida");

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                                  Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(Token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Token Invalido");
        }

        return principal;
    }
}
