using Pizzeria.API.DTOs;
using Pizzeria.API.Models;

namespace Pizzeria.API.Services.IServices;
public interface IPizzaService
{
    Task<List<Pizza>> ObtenerTodasAsync();

    Task<Pizza?> ObtenerPorIdAsync(int id);

    Task<Pizza> CrearAsync(CreatePizzaDto dto);

    Task<int> ActualizarAsync(int id, CreatePizzaDto dto);

    Task<int> EliminarAsync(int id);
}