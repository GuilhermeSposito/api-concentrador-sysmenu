using System.ComponentModel.DataAnnotations;

namespace ConcentradorSysMenu.DTOs;

public class RegisterModel
{
    [Required(ErrorMessage = "O Email é obrigatório")] public string? Email { get; set; }
    [Required(ErrorMessage = "O Nome é obrigatório")] public string? Nome { get; set; }
    [Required(ErrorMessage = "O Documento (Cnpj/CPF) é obrigatório")] public string? Doc { get; set; }
    [Required(ErrorMessage = "A Senha é obrigatória")] public string? Password { get; set; }
}
