using System.ComponentModel.DataAnnotations;

namespace ConcentradorSysMenu.DTOs;

public class LoginModel
{
    [Required(ErrorMessage = "O Email é obrigatório")]public string? Email { get; set; }
    [Required(ErrorMessage = "A Senha é obrigatória")]public string? Password { get; set; }
}
