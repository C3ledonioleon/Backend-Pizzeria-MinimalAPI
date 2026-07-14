using Pizzeria.API.DTOs;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Services;

public class ClienteService
{
    private readonly List<Cliente> _clientes = new();
    private int _siguienteId = 1;

    public List<Cliente> ObtenerTodos() => _clientes;

    public Cliente? ObtenerPorId(int id) =>
        _clientes.FirstOrDefault(c => c.IdCliente == id);

    public Cliente Crear(CreateClienteDto dto)
    {
        var cliente = new Cliente(dto.Nombre, dto.Apellido, dto.Email, dto.Telefono, dto.Direccion)
        {
            IdCliente = _siguienteId++
        };
        _clientes.Add(cliente);
        return cliente;
    }
}