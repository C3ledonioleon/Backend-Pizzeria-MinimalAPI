using Dapper;
using Pizzeria.API.Data;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClienteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Cliente>> ObtenerTodosAsync()
    {
        using var connection = _connectionFactory.CrearConexion();
        var clientes = await connection.QueryAsync<Cliente>("SELECT * FROM Cliente");
        return clientes.ToList();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();
        return await connection.QuerySingleOrDefaultAsync<Cliente>(
            "SELECT * FROM Cliente WHERE IdCliente = @Id", new { Id = id });
    }

    public async Task<Cliente> CrearAsync(Cliente cliente)
    {
        using var connection = _connectionFactory.CrearConexion();

        const string sql = @"
            INSERT INTO Cliente (Nombre, Apellido, Email, Telefono, Direccion)
            VALUES (@Nombre, @Apellido, @Email, @Telefono, @Direccion);
            SELECT LAST_INSERT_ID();";

        var nuevoId = await connection.QuerySingleAsync<int>(sql, cliente);
        cliente.IdCliente = nuevoId;
        return cliente;
    }

    public async Task<int> ActualizarAsync(int id, Cliente cliente)
    {
        using var connection = _connectionFactory.CrearConexion();

        const string sql = @"
            UPDATE Cliente
            SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email,
                Telefono = @Telefono, Direccion = @Direccion
            WHERE IdCliente = @Id";

        return await connection.ExecuteAsync(sql, new
        {
            cliente.Nombre,
            cliente.Apellido,
            cliente.Email,
            cliente.Telefono,
            cliente.Direccion,
            Id = id
        });
    }

    public async Task<int> EliminarAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();
        return await connection.ExecuteAsync(
            "DELETE FROM Cliente WHERE IdCliente = @Id", new { Id = id });
    }
}