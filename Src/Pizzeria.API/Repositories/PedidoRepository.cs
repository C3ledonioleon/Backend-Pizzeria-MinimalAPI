using Dapper;
using Pizzeria.API.Data;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PedidoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Pedido>> ObtenerTodosAsync()
    {
        using var connection = _connectionFactory.CrearConexion();

        return (await connection.QueryAsync<Pedido>(
            "SELECT * FROM Pedido")).ToList();
    }

    public async Task<Pedido?> ObtenerPorIdAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();

        return await connection.QueryFirstOrDefaultAsync<Pedido>(
            "SELECT * FROM Pedido WHERE IdPedido = @IdPedido",
            new { IdPedido = id });
    }

    public async Task<Pedido> CrearAsync(Pedido pedido)
    {
        using var connection = _connectionFactory.CrearConexion();

        string sql = @"
        INSERT INTO Pedido (FechaHora, Estado, IdCliente, IdSucursal, IdEmpleado, Total)
        VALUES (@FechaHora, @Estado, @IdCliente, @IdSucursal, @IdEmpleado, @Total);
        SELECT LAST_INSERT_ID();";

        int nuevoId = await connection.ExecuteScalarAsync<int>(sql, pedido);

        pedido.IdPedido = nuevoId;

        return pedido;
    }

    public async Task<int> ActualizarAsync(int id, Pedido pedido)
    {
        using var connection = _connectionFactory.CrearConexion();

        string sql = @"
        UPDATE Pedido
        SET FechaHora = @FechaHora,
            Estado = @Estado,
            IdCliente = @IdCliente,
            IdSucursal = @IdSucursal,
            IdEmpleado = @IdEmpleado,
            Total = @Total
        WHERE IdPedido = @IdPedido";

        return await connection.ExecuteAsync(sql, new
        {
            pedido.FechaHora,
            pedido.Estado,
            pedido.IdCliente,
            pedido.IdSucursal,
            pedido.IdEmpleado,
            pedido.Total,
            IdPedido = id
        });
    }

    public async Task<int> EliminarAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();

        string sql = "DELETE FROM Pedido WHERE IdPedido = @IdPedido";

        return await connection.ExecuteAsync(sql, new { IdPedido = id });
    }

    public async Task ActualizarEstadoAsync(int idPedido, int nuevoEstado)
    {
        using var connection = _connectionFactory.CrearConexion();

        await connection.ExecuteAsync(
            "UPDATE Pedido SET Estado = @Estado WHERE IdPedido = @IdPedido",
            new { Estado = nuevoEstado, IdPedido = idPedido });
    }

    public async Task AsignarEmpleadoAsync(int idPedido, int idEmpleado)
    {
        using var connection = _connectionFactory.CrearConexion();

        await connection.ExecuteAsync(
            "UPDATE Pedido SET IdEmpleado = @IdEmpleado WHERE IdPedido = @IdPedido",
            new { IdEmpleado = idEmpleado, IdPedido = idPedido });
    }
}