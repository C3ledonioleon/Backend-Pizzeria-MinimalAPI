using Pizzeria.API.DTOs;
using Pizzeria.API.Enums;
using Pizzeria.API.Services.IServices;

namespace Pizzeria.API.Endpoints;
public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints(this WebApplication app)
    {
        var pedido = app.MapGroup("/api/pedidos");

        pedido.WithTags("Pedido");


        pedido.MapGet("/", async ( IPedidoService service) =>
        {
            var pedidos = await service.ObtenerTodosAsync();

            return Results.Ok(pedidos);
        });

        pedido.MapGet("/{id:int}", async (int id, IPedidoService service) =>
        {
            var pedido = await service.ObtenerPorIdAsync(id);

            return pedido is null
                ? Results.NotFound()
                : Results.Ok(pedido);
        });

        pedido.MapPost("/", async ( CreatePedidoDto dto, IPedidoService service) =>
        {
            var pedido = await service.CrearAsync(dto);

            return Results.Created(
                $"/api/pedidos/{pedido.IdPedido}",
                pedido);
        });

        pedido.MapPut("/{id:int}/estado", async ( int id,EstadoPedido estado, IPedidoService service) =>
        {
            var pedido = await service.CambiarEstadoAsync(id, estado);

            return pedido is null
                ? Results.NotFound()
                : Results.Ok(pedido);
        });

        pedido.MapDelete("/{id:int}", async ( int id,IPedidoService service) =>
        {
            var eliminado = await service.EliminarAsync(id);

            return eliminado > 0
                ? Results.Ok()
                : Results.NotFound();
        });

        // Detalles del pedido

        pedido.MapPost("/{idPedido:int}/detalles", async ( int idPedido, DetallePedidoDto dto, IPedidoService service) =>
            {
                var pedido = await service.AgregarDetalleAsync(idPedido, dto);

                return pedido is null
                    ? Results.NotFound()
                    : Results.Ok(pedido);
            });

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
            });

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
            });
    }
}