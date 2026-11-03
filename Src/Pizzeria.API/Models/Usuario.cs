using Pizzeria.API.Enums;

namespace Pizzeria.API.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public RolUsuario Rol { get; set; }

        public int? IdCliente { get; set; }

        public Usuario() { }

        public Usuario(string email, string password, RolUsuario rol)
        {
            Email = email;
            Password = password;
            Rol = rol;
        }
    }
}