
using Pizzeria.API.Models;

namespace Pizzeria.API.Repositories.IRepositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorIdAsync(int idUsuario);

        Task<Usuario?> ObtenerPorUsuarioOEmailAsync(string usuarioOEmail);

        Task<Usuario> RegistrarAsync(Usuario usuario);

        Task ActualizarContrasenaAsync(int idUsuario, string passwordHash);
    }
}
