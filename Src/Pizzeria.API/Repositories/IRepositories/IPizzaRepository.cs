using Pizzeria.API.Models;

namespace Pizzeria.API.Repositories.IRepositories;

public interface IPizzaRepository
{
    Task<List<Pizza>> ObtenerTodasAsync();
    Task<Pizza?> ObtenerPorIdAsync(int id);
    Task<Pizza> CrearAsync(Pizza pizza);
    Task<int> ActualizarAsync(int id, Pizza pizza);
    Task<int> EliminarAsync(int id);
}