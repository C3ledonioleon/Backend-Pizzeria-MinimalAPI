using Pizzeria.API.DTOs;
using Pizzeria.API.Models;

namespace Pizzeria.API.Services.IServices;

public interface IClienteService
{
    Task<List<Cliente>> ObtenerTodosAsync();

    Task<Cliente?> ObtenerPorIdAsync(int id);

    Task<Cliente> CrearAsync(CreateClienteDto dto);

    Task<bool> ActualizarAsync(int id, UpdateClienteDto dto);

    Task<bool> EliminarAsync(int id);
}