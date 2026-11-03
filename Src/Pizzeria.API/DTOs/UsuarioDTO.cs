using Pizzeria.API.Enums;

namespace Pizzeria.API.DTOs;

    public class RegistrarUsuarioDTO
    {
    
        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public string Contrasena { get; set; } = string.Empty;
 }
public class LoginDTO
{
    public string Email { get; set; } 
    public string Contrasena { get; set; } 
}
    


    public class UsuarioRespuestaDTO
 {
        public int IdUsuario { get; set; }

        public int? IdCliente { get; set; }

        public string Email { get; set; } = string.Empty;

        public RolUsuario Rol { get; set; }
 }
