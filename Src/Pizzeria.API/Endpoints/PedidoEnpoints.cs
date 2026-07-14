using Pizzeria.API.DTOs;
using Pizzeria.API.Enums;
using Pizzeria.API.Services;
using Pizzeria.API.Validators;

namespace Pizzeria.API.Endpoints;

public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/pedidos");

        grupo.MapGet("/", async (PedidoService service) =>
            Results.Ok(await service.ObtenerTodosAsync()));

        grupo.MapGet("/{id}", async (int id, PedidoService service) =>
        {
            var pedido = await service.ObtenerPorIdAsync(id);
            return pedido is not null ? Results.Ok(pedido) : Results.NotFound();
        });

        grupo.MapPost("/", async (CreatePedidoDto dto, PedidoService service) =>
        {
            var errores = PedidoValidator.Validar(dto);
            if (errores.Count > 0)
                return Results.BadRequest(errores);

            try
            {
                var pedido = await service.CrearAsync(dto);
                return Results.Created($"/pedidos/{pedido.IdPedido}", pedido);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        grupo.MapPut("/{id}/estado", async (int id, EstadoPedido nuevoEstado, PedidoService service) =>
        {
            var pedido = await service.CambiarEstadoAsync(id, nuevoEstado);
            return pedido is not null ? Results.Ok(pedido) : Results.NotFound();
        });

        grupo.MapPut("/{id}/asignar-empleado", async (int id, int idEmpleado, PedidoService service) =>
        {
            var pedido = await service.AsignarEmpleadoAsync(id, idEmpleado);
            return pedido is not null ? Results.Ok(pedido) : Results.NotFound();
        });

        grupo.MapDelete("/{id}", async (int id, PedidoService service) =>
        {
            var filasAfectadas = await service.EliminarAsync(id);
            return filasAfectadas > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}