using Pizzeria.API.Models;

namespace Pizzeria.API.Repositories.IRepositories;

public interface IClienteRepository
{
    Task<List<Cliente>> ObtenerTodosAsync();
    Task<Cliente?> ObtenerPorIdAsync(int id);
    Task<Cliente> CrearAsync(Cliente cliente);
    Task<int> ActualizarAsync(int id, Cliente cliente);
    Task<int> EliminarAsync(int id);
}