using Pizzeria.API.Models;

namespace Pizzeria.API.Repositories.IRepositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorUsuarioOEmailAsync(string usuarioOEmail);

        Task<Usuario> RegistrarAsync(Usuario usuario);
    }
}