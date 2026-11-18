
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

    public async Task<List<Pedido>> ObtenerPorUsuarioAsync(int idUsuario)
    {
        using var connection = _connectionFactory.CrearConexion();

        const string sql = @"
            SELECT p.*
            FROM Pedido p
            INNER JOIN Cliente c ON c.IdCliente = p.IdCliente
            WHERE c.IdUsuario = @IdUsuario
            ORDER BY p.FechaHora DESC";

        return (await connection.QueryAsync<Pedido>(
            sql, new { IdUsuario = idUsuario })).ToList();
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

        const string sql = @"
            INSERT INTO Pedido
            (FechaHora, Estado, MedioPago, IdCliente, Total)
            VALUES
            (@FechaHora, @Estado, @MedioPago, @IdCliente, @Total);

            SELECT LAST_INSERT_ID();";

        int nuevoId = await connection.ExecuteScalarAsync<int>(sql, new
        {
            pedido.FechaHora,
            pedido.Estado,
            MedioPago = pedido.MedioPago.HasValue
                ? (int?)pedido.MedioPago.Value
                : null,
            pedido.IdCliente,
            pedido.Total
        });

        pedido.IdPedido = nuevoId;

        return pedido;
    }

    public async Task<int> ActualizarAsync(int id, Pedido pedido)
    {
        using var connection = _connectionFactory.CrearConexion();

        const string sql = @"
            UPDATE Pedido
            SET FechaHora = @FechaHora,
                Estado = @Estado,
                MedioPago = @MedioPago,
                IdCliente = @IdCliente,
                Total = @Total
            WHERE IdPedido = @IdPedido";

        return await connection.ExecuteAsync(sql, new
        {
            pedido.FechaHora,
            pedido.Estado,
            MedioPago = pedido.MedioPago.HasValue
                ? (int?)pedido.MedioPago.Value
                : null,
            pedido.IdCliente,
            pedido.Total,
            IdPedido = id
        });
    }

    public async Task<int> EliminarAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();

        return await connection.ExecuteAsync(
            "DELETE FROM Pedido WHERE IdPedido = @IdPedido",
            new { IdPedido = id });
    }

    public async Task ActualizarEstadoAsync(int idPedido, int nuevoEstado)
    {
        using var connection = _connectionFactory.CrearConexion();

        await connection.ExecuteAsync(
            "UPDATE Pedido SET Estado = @Estado WHERE IdPedido = @IdPedido",
            new { Estado = nuevoEstado, IdPedido = idPedido });
    }
}
