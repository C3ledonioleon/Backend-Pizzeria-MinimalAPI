using Pizzeria.API.Models;
namespace Pizzeria.API.Repositories.IRepositories;
public interface IEmpleadoRepository
{
    Task<List<Empleado>> ObtenerTodosAsync();
    Task<Empleado?> ObtenerPorIdAsync(int id);
    Task<Empleado> CrearAsync(Empleado empleado);
    Task<int> ActualizarAsync(int id, Empleado empleado);
    Task<int> EliminarAsync(int id);
}