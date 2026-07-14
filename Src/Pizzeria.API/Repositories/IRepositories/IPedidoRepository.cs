using Pizzeria.API.Models;
namespace Pizzeria.API.Repositories.IRepositories;

public interface IPedidoRepository
{
    Task<List<Pedido>> ObtenerTodosAsync();
    Task<Pedido?> ObtenerPorIdAsync(int id);
    Task<Pedido> CrearAsync(Pedido pedido);
    Task<int> ActualizarAsync(int id, Pedido pedido);
    Task<int> EliminarAsync(int id);
    Task ActualizarEstadoAsync(int idPedido, int nuevoEstado);
    Task AsignarEmpleadoAsync(int idPedido, int idEmpleado);
}