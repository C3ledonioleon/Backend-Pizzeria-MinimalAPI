using Dapper;
using Pizzeria.API.Data;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Repositories;

public class SucursalRepository : ISucursalRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SucursalRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Sucursal>> ObtenerTodasAsync()
    {
        using var connection = _connectionFactory.CrearConexion();
        var sucursales = await connection.QueryAsync<Sucursal>("SELECT * FROM Sucursal");
        return sucursales.ToList();
    }

    public async Task<Sucursal?> ObtenerPorIdAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();
        return await connection.QuerySingleOrDefaultAsync<Sucursal>(
            "SELECT * FROM Sucursal WHERE IdSucursal = @Id", new { Id = id });
    }

    public async Task<Sucursal> CrearAsync(Sucursal sucursal)
    {
        using var connection = _connectionFactory.CrearConexion();

        const string sql = @"
            INSERT INTO Sucursal (Nombre, Direccion, Telefono, Activa)
            VALUES (@Nombre, @Direccion, @Telefono, @Activa);
            SELECT LAST_INSERT_ID();";

        var nuevoId = await connection.QuerySingleAsync<int>(sql, sucursal);
        sucursal.IdSucursal = nuevoId;
        return sucursal;
    }

    public async Task<int> ActualizarAsync(int id, Sucursal sucursal)
    {
        using var connection = _connectionFactory.CrearConexion();

        const string sql = @"
            UPDATE Sucursal
            SET Nombre = @Nombre, Direccion = @Direccion,
                Telefono = @Telefono, Activa = @Activa
            WHERE IdSucursal = @Id";

        return await connection.ExecuteAsync(sql, new
        {
            sucursal.Nombre,
            sucursal.Direccion,
            sucursal.Telefono,
            sucursal.Activa,
            Id = id
        });
    }

    public async Task<int> EliminarAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();
        return await connection.ExecuteAsync(
            "DELETE FROM Sucursal WHERE IdSucursal = @Id", new { Id = id });
    }
}