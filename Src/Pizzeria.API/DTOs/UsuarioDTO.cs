using Pizzeria.API.Enums;

namespace Pizzeria.API.DTOs
{
    public class RegistrarUsuarioDTO
    {
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Contrasena { get; set; } = string.Empty;
    }


    public class LoginDTO
    {
            public string UsuarioOEmail { get; set; } = string.Empty;

            public string Contrasena { get; set; } = string.Empty;
    }

    


    public class UsuarioRespuestaDTO
 {
        public int IdUsuario { get; set; }
        public int? IdCliente { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }
 }

}
