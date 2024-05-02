using System.ComponentModel.DataAnnotations;

namespace CatalogAPI.DTOs
{
    public class LoginModelDTO
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório!")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória!")]
        public string? Password { get; set; }
    }
}
