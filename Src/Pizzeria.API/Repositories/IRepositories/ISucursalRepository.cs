using Pizzeria.API.Models;
namespace Pizzeria.API.Repositories.IRepositories;

public interface ISucursalRepository
{
    Task<List<Sucursal>> ObtenerTodasAsync();
    Task<Sucursal?> ObtenerPorIdAsync(int id);
    Task<Sucursal> CrearAsync(Sucursal sucursal);
    Task<int> ActualizarAsync(int id, Sucursal sucursal);
    Task<int> EliminarAsync(int id);
}