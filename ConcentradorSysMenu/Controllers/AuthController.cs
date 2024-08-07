using ConcentradorSysMenu.DTOs;
using ConcentradorSysMenu.Models;
using ConcentradorSysMenu.Services.TokenServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ConcentradorSysMenu.Controllers;

[Route("/ConSysMenu/api/v1/[controller]")]
[ApiController]
public class AuthController : Controller
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AuthController> _Logger;
    private readonly RoleManager<IdentityRole> _RoleManeger;
    private readonly IConfiguration _configuration;

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager, ILogger<AuthController> logger, RoleManager<IdentityRole> roleManeger, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _Logger = logger;
        _RoleManeger = roleManeger;
        _configuration = configuration;
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
    {
        var user = await _userManager.FindByEmailAsync(loginModel.Email!);

        if (user is null || await _userManager.CheckPasswordAsync(user, loginModel.Password!) == false)
            return Unauthorized(new Response { Status = false, Messages = new List<string> { "Usuario ou senha incorretos" } });

        var UserRoles = await _userManager.GetRolesAsync(user);

        var AuthClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim("id", user.Id),
            new Claim("doc", user.Doc!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var Token = _tokenService.GeraToken(AuthClaims, _configuration);
        var RefreshToken = _tokenService.GeraRefreshToken();

        _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int ValidTokenInMinuts);

        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(ValidTokenInMinuts);
        user.RefreshToken = RefreshToken;

        await _userManager.UpdateAsync(user);

        return Ok(new
        {
            response = new Response()
            {
                Status = true,
                Messages = new List<string>
                {
                "Logado com sucesso"
                }
            },
            infos = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(Token),
                refreshToken = RefreshToken,
                exp = Token.ValidTo
            }
        }
        );
    }

    [HttpPost("RegistraUsuario")]
    public async Task<ActionResult> RegistraUsuario([FromBody] RegisterModel UserModel)
    {
        var existUser = await _userManager.FindByEmailAsync(UserModel.Email!);

        if (existUser is not null)
            return BadRequest(new Response()
            {
                Status = false,
                Messages = new List<string>
                {
                    "Usuario já cadastrado!"
                }
            });

        ApplicationUser newUser = new ApplicationUser()
        {
            Email = UserModel.Email,
            Doc = UserModel.Doc,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = UserModel.Nome
        };

        var InsertResult = await _userManager.CreateAsync(newUser, UserModel.Password!);

        if (!InsertResult.Succeeded)
        {
            Response response = new Response()
            {
                Status = false,
                Messages = new List<string>()
            };

            foreach (var erro in InsertResult.Errors)
            {
                response.Messages.Add(erro.Description!);
            }

            return BadRequest(response);
        }

        return StatusCode(statusCode: StatusCodes.Status201Created, new Response
        {
            Status = true,
            Messages = new List<string>
            {
                "Usuário Criado com sucesso"
            }
        });

    }

   
}
