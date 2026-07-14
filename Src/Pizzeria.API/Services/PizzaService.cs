using Pizzeria.API.DTOs;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Services;

public class PizzaService
{
    private readonly IPizzaRepository _repository;

    public PizzaService(IPizzaRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Pizza>> ObtenerTodasAsync() => _repository.ObtenerTodasAsync();

    public Task<Pizza?> ObtenerPorIdAsync(int id) => _repository.ObtenerPorIdAsync(id);

    public Task<Pizza> CrearAsync(CreatePizzaDto dto)
    {
        var pizza = new Pizza(dto.Nombre, dto.Precio, dto.Descripcion)
        {
            Ingredientes = dto.Ingredientes
        };
        return _repository.CrearAsync(pizza);
    }

    public Task<int> ActualizarAsync(int id, CreatePizzaDto dto)
    {
        var pizza = new Pizza(dto.Nombre, dto.Precio, dto.Descripcion)
        {
            Ingredientes = dto.Ingredientes
        };
        return _repository.ActualizarAsync(id, pizza);
    }

    public Task<int> EliminarAsync(int id) => _repository.EliminarAsync(id);
}