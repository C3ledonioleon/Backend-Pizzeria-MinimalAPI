using Pizzeria.API.DTOs;
using Pizzeria.API.Enums;
using Pizzeria.API.Models;

namespace Pizzeria.API.Services.IServices;

public interface IPedidoService
{
    Task<List<Pedido>> ObtenerTodosAsync();

    Task<Pedido?> ObtenerPorIdAsync(int id);

    Task<Pedido> CrearAsync(CreatePedidoDto dto);

    Task<Pedido?> CambiarEstadoAsync(int idPedido, EstadoPedido nuevoEstado);

    Task<int> EliminarAsync(int id);

    Task<Pedido?> AgregarDetalleAsync(int idPedido, DetallePedidoDto dto);

    Task<Pedido?> ActualizarDetalleAsync(
        int idPedido,
        int idDetalle,
        ActualizarDetalleDto dto);

    Task<Pedido?> EliminarDetalleAsync(
        int idPedido,
        int idDetalle);
}