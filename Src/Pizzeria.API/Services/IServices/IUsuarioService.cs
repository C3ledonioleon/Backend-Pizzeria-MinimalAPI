using Pizzeria.API.DTOs;

namespace Pizzeria.API.Services.IServices
{
    public interface IUsuarioService
    {
        Task<UsuarioRespuestaDTO> RegistrarAsync(
            RegistrarUsuarioDTO dto
        );

        Task<UsuarioRespuestaDTO?> LoginAsync(
            LoginDTO dto
        );
    }
}
