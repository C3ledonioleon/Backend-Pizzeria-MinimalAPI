using Pizzeria.API.Enums;

namespace Pizzeria.API.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public RolUsuario Rol { get; set; }

        public int? IdCliente { get; set; }

        public Usuario() { }

        public Usuario(string username, string email, string password, RolUsuario rol)
        {
            Username = username;
            Email = email;
            Password = password;
            Rol = rol;
        }
    }
}