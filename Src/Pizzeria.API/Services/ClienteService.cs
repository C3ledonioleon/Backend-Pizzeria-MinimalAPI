using Pizzeria.API.DTOs;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;
using Pizzeria.API.Services.IServices;

namespace Pizzeria.API.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }


    public async Task<List<Cliente>> ObtenerTodosAsync()
    {
        return await _repository.ObtenerTodosAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await _repository.ObtenerPorIdAsync(id);
    }

    public async Task<Cliente> CrearAsync(CreateClienteDto dto)
    {
        var cliente = new Cliente(
            dto.Nombre,
            dto.Apellido,
            dto.Email,
            dto.Telefono,
            dto.Direccion
        );

        return await _repository.CrearAsync(cliente);
    }

    public async Task<bool> ActualizarAsync(int id, UpdateClienteDto dto)
    {
        var cliente = await _repository.ObtenerPorIdAsync(id);

        if (cliente is null)
            return false;

        cliente.Email = dto.Email;
        cliente.Telefono = dto.Telefono;
        cliente.Direccion = dto.Direccion;

        var filas = await _repository.ActualizarAsync(id, cliente);

        return filas > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var filas = await _repository.EliminarAsync(id);

        return filas > 0;
    }
}