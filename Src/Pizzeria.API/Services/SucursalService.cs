using Pizzeria.API.DTOs;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Services;

public class SucursalService
{
    private readonly ISucursalRepository _repository;

    public SucursalService(ISucursalRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Sucursal>> ObtenerTodasAsync() => _repository.ObtenerTodasAsync();

    public Task<Sucursal?> ObtenerPorIdAsync(int id) => _repository.ObtenerPorIdAsync(id);

    public Task<Sucursal> CrearAsync(CreateSucursalDto dto)
    {
        var sucursal = new Sucursal(dto.Nombre, dto.Direccion, dto.Telefono);
        return _repository.CrearAsync(sucursal);
    }

    public Task<int> ActualizarAsync(int id, CreateSucursalDto dto)
    {
        var sucursal = new Sucursal(dto.Nombre, dto.Direccion, dto.Telefono);
        return _repository.ActualizarAsync(id, sucursal);
    }

    public Task<int> EliminarAsync(int id) => _repository.EliminarAsync(id);
}