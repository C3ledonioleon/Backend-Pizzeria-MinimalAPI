using Pizzeria.API.Models;

namespace Pizzeria.API.Repositories.IRepositories;

public interface IDetallePedidoRepository
{
    Task<List<DetallePedido>> ObtenerPorPedidoAsync(int idPedido);
    Task<DetallePedido?> ObtenerPorIdAsync(int idDetalle);
    Task<DetallePedido> AgregarAsync(DetallePedido detalle);
    Task<int> ActualizarAsync(int idDetalle, int cantidad, string? observaciones);
    Task<int> EliminarAsync(int idDetalle);
}