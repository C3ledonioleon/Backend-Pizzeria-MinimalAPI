
using System.ComponentModel.DataAnnotations;

namespace MVC.Pizzeria.Models
{
    // MODELO PARA INICIAR SESIÓN
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingresá tu usuario o email.")]
        public string UsuarioOEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá tu contraseña.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;
    }

    // MODELO PARA REGISTRARSE
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "Ingresá un nombre de usuario.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá tu email.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá una contraseña.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;
    }

    // RESPUESTA QUE RECIBIMOS DE LA API
    public class UsuarioRespuestaMVC
    {
        public int IdUsuario { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}
