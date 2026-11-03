using Pizzeria.API.Models;

namespace Pizzeria.API.Repositories.IRepositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorEmailAsync(string email);

        Task<Usuario> RegistrarClienteAsync(
            Usuario usuario,
            Cliente cliente
        );
    }
}
