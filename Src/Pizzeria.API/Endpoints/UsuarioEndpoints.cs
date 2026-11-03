using Pizzeria.API.DTOs;
using Pizzeria.API.Services.IServices;

namespace Pizzeria.API.Endpoints
{
    public static class UsuarioEndpoints
    {
        public static void MapUsuarioEndpoints(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/usuarios");

            // REGISTRO
            grupo.MapPost("/registro", async (
                RegistrarUsuarioDTO dto,
                IUsuarioService usuarioService) =>
            {
                try
                {
                    var usuario = await usuarioService.RegistrarAsync(dto);

                    return Results.Created(
                        $"/api/usuarios/{usuario.IdUsuario}",
                        usuario
                    );
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new
                    {
                        mensaje = ex.Message
                    });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new
                    {
                        mensaje = ex.Message
                    });
                }
            });

            // LOGIN
            grupo.MapPost("/login", async (
                LoginDTO dto,
                IUsuarioService usuarioService) =>
            {
                var usuario = await usuarioService.LoginAsync(dto);

                if (usuario == null)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(usuario);
            });
        }
    }
}
