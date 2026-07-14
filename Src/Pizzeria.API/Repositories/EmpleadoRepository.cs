using Dapper;
using Pizzeria.API.Data;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Repositories;

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EmpleadoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Empleado>> ObtenerTodosAsync()
    {
        using var connection = _connectionFactory.CrearConexion();

        return (await connection.QueryAsync<Empleado>(
            "SELECT * FROM Empleado")).ToList();
    }

    public async Task<Empleado?> ObtenerPorIdAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();

        return await connection.QueryFirstOrDefaultAsync<Empleado>(
            "SELECT * FROM Empleado WHERE IdEmpleado = @IdEmpleado",
            new { IdEmpleado = id });
    }

    public async Task<Empleado> CrearAsync(Empleado empleado)
    {
        using var connection = _connectionFactory.CrearConexion();

        string sql = @"
        INSERT INTO Empleado (Nombre, Apellido, Rol, DNI, Telefono)
        VALUES (@Nombre, @Apellido, @Rol, @DNI, @Telefono);
        SELECT LAST_INSERT_ID();";

        int nuevoId = await connection.ExecuteScalarAsync<int>(sql, empleado);

        empleado.IdEmpleado = nuevoId;

        return empleado;
    }

    public async Task<int> ActualizarAsync(int id, Empleado empleado)
    {
        using var connection = _connectionFactory.CrearConexion();

        string sql = @"
        UPDATE Empleado
        SET Nombre = @Nombre,
            Apellido = @Apellido,
            Rol = @Rol,
            DNI = @DNI,
            Telefono = @Telefono
        WHERE IdEmpleado = @IdEmpleado";

        int filas = await connection.ExecuteAsync(sql, new
        {
            empleado.Nombre,
            empleado.Apellido,
            empleado.Rol,
            empleado.DNI,
            empleado.Telefono,
            IdEmpleado = id
        });

        return filas;
    }

    public async Task<int> EliminarAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();

        string sql = "DELETE FROM Empleado WHERE IdEmpleado = @IdEmpleado";

        return await connection.ExecuteAsync(sql, new { IdEmpleado = id });
    }
}