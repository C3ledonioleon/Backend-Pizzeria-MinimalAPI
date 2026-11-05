using Pizzeria.API.DTOs;
using Pizzeria.API.Models;
using Pizzeria.API.Services.IServices;

namespace Pizzeria.API.Endpoints
{
    public static class UsuarioEndpoints
    {
        public static void MapUsuarioEndpoints(this WebApplication app)
        {
            var usuario = app.MapGroup("/api/usuarios");

          usuario.WithTags("Usuario");

        // REGISTRO
        usuario.MapPost("/registro", async (
            RegistrarUsuarioDTO dto,
            IUsuarioService usuarioService) =>
        {
            try
            {
                var resultado = await usuarioService.RegistrarAsync(dto);

                return Results.Created(
                    $"/api/usuarios/{resultado.IdUsuario}",
                    resultado
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
        usuario.MapPost("/login", async (
            LoginDTO dto,
            IUsuarioService usuarioService) =>
        {
            try
            {
                var resultado = await usuarioService.LoginAsync(dto);

                if (resultado == null)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        });
    }
}
}