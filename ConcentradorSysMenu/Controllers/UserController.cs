using ConcentradorSysMenu.DTOs;
using ConcentradorSysMenu.Models;
using ConcentradorSysMenu.Services.TokenServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConcentradorSysMenu.Controllers;

[Route("/ConSysMenu/api/v1/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(ITokenService tokenService, UserManager<ApplicationUser> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    [HttpGet("ObterUsuario")]
    [Authorize]
    public async Task<ActionResult> ObterUsuario()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;

        if (claimsIdentity is null)
            return Unauthorized(new Response()
            {
                Status = false,
                Messages = new List<string>
            {
                "Usuario não autenticado!"
            }
            });
        var Claims = claimsIdentity.Claims;

        var emailClaim = Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var user = await _userManager.FindByEmailAsync(emailClaim!);

        return Ok(new
        {
            response = new Response()
            {
                Status = true,
                Messages = new List<string>
                {
                }
            },
            infos = new
            {
                Email = user.Email,
                Nome = user.UserName,
                id = user.Id,
                doc = user.Doc
            }
        }
       );
    }
}
