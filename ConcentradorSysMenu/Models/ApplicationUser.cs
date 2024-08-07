using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ConcentradorSysMenu.Models;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    [Required][StringLength(50)]public string? Doc { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
