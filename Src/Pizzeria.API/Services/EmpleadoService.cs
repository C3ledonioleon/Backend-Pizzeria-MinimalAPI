using Pizzeria.API.DTOs;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Services;

public class EmpleadoService
{
    private readonly IEmpleadoRepository _repository;

    public EmpleadoService(IEmpleadoRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Empleado>> ObtenerTodosAsync() => _repository.ObtenerTodosAsync();

    public Task<Empleado?> ObtenerPorIdAsync(int id) => _repository.ObtenerPorIdAsync(id);

    public Task<Empleado> CrearAsync(CreateEmpleadoDto dto)
    {
        var empleado = new Empleado(dto.Nombre, dto.Apellido, dto.Rol, dto.DNI, dto.Telefono)
        {
            IdsSucursales = dto.IdsSucursales
        };
        return _repository.CrearAsync(empleado);
    }

    public Task<int> ActualizarAsync(int id, CreateEmpleadoDto dto)
    {
        var empleado = new Empleado(dto.Nombre, dto.Apellido, dto.Rol, dto.DNI, dto.Telefono)
        {
            IdsSucursales = dto.IdsSucursales
        };
        return _repository.ActualizarAsync(id, empleado);
    }

    public Task<int> EliminarAsync(int id) => _repository.EliminarAsync(id);
}