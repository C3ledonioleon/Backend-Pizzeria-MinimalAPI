
using Pizzeria.API.DTOs;
using Pizzeria.API.Enums;
using Pizzeria.API.Services.IServices;
using System.Security.Claims;

namespace Pizzeria.API.Endpoints;

public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints(this WebApplication app)
    {
        var pedido = app.MapGroup("/api/pedidos");

        pedido.WithTags("Pedido");

        // Obtener todos los pedidos (Administrador).
        pedido.MapGet("/", async (IPedidoService service) =>
        {
            var pedidos = await service.ObtenerTodosAsync();
            return Results.Ok(pedidos);

        }).RequireAuthorization(policy =>
            policy.RequireRole("Administrador"));

        // Crear un pedido (Cliente).
        pedido.MapPost("/", async (
            CreatePedidoDto dto,
            ClaimsPrincipal usuario,
            IPedidoService service) =>
        {
            var idUsuarioClaim =
                usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            try
            {
                var pedidoCreado = await service.CrearAsync(idUsuario, dto);

                return Results.Created(
                    $"/api/pedidos/{pedidoCreado.IdPedido}",
                    pedidoCreado);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { mensaje = ex.Message });
            }

        }).RequireAuthorization(policy =>
            policy.RequireRole("Cliente"));

        // Obtener los pedidos del cliente autenticado.
        pedido.MapGet("/mis-pedidos", async (
            ClaimsPrincipal usuario,
            IPedidoService service) =>
        {
            var idUsuarioClaim =
                usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var pedidos = await service.ObtenerPorUsuarioAsync(idUsuario);

            return Results.Ok(pedidos);

        }).RequireAuthorization(policy =>
            policy.RequireRole("Cliente"));

        // Obtener un pedido específico (Administrador).
        pedido.MapGet("/{id:int}", async (
            int id,
            IPedidoService service) =>
        {
            var pedido = await service.ObtenerPorIdAsync(id);

            return pedido is null
                ? Results.NotFound()
                : Results.Ok(pedido);

        }).RequireAuthorization(policy =>
            policy.RequireRole("Administrador"));

        // Cambiar estado (Administrador).
        pedido.MapPut("/{id:int}/estado", async (
            int id,
            EstadoPedido estado,
            IPedidoService service) =>
        {
            if (!Enum.IsDefined(estado))
            {
                return Results.BadRequest(new
                {
                    mensaje = "El estado del pedido no es válido."
                });
            }

            var pedido = await service.CambiarEstadoAsync(id, estado);

            return pedido is null
                ? Results.NotFound()
                : Results.Ok(pedido);

        }).RequireAuthorization(policy =>
            policy.RequireRole("Administrador"));

        // Eliminar pedido (Administrador).
        pedido.MapDelete("/{id:int}", async (
            int id,
            IPedidoService service) =>
        {
            var eliminado = await service.EliminarAsync(id);

            return eliminado > 0
                ? Results.Ok()
                : Results.NotFound();

        }).RequireAuthorization(policy =>
            policy.RequireRole("Administrador"));

        pedido.MapPost("/{idPedido:int}/detalles", async ( int idPedido, DetallePedidoDto dto, IPedidoService service) =>
            {
                var pedido = await service.AgregarDetalleAsync(idPedido, dto);

                return pedido is null
                    ? Results.NotFound()
                    : Results.Ok(pedido);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));

        pedido.MapPut("/{idPedido:int}/detalles/{idDetalle:int}",
            async (
                int idPedido,
                int idDetalle,
                ActualizarDetalleDto dto,
                IPedidoService service) =>
            {
                var pedido = await service.ActualizarDetalleAsync( idPedido,idDetalle,dto);

                return pedido is null
                    ? Results.NotFound()
                    : Results.Ok(pedido);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));

        pedido.MapDelete("/{idPedido:int}/detalles/{idDetalle:int}",
            async (
                int idPedido,
                int idDetalle,
                IPedidoService service) =>
            {
                var pedido = await service.EliminarDetalleAsync(
                    idPedido,
                    idDetalle);

                return pedido is null
                    ? Results.NotFound()
                    : Results.Ok(pedido);
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));
    }
}
