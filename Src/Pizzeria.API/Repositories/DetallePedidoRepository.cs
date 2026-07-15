using Dapper;
using Pizzeria.API.Data;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Repositories;

public class DetallePedidoRepository : IDetallePedidoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DetallePedidoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<DetallePedido>> ObtenerPorPedidoAsync(int idPedido)
    {
        using var connection = _connectionFactory.CrearConexion();
        var detalles = await connection.QueryAsync<DetallePedido>(
            "SELECT * FROM DetallePedido WHERE IdPedido = @IdPedido", new { IdPedido = idPedido });
        return detalles.ToList();
    }

    public async Task<DetallePedido?> ObtenerPorIdAsync(int idDetalle)
    {
        using var connection = _connectionFactory.CrearConexion();
        return await connection.QuerySingleOrDefaultAsync<DetallePedido>(
            "SELECT * FROM DetallePedido WHERE IdDetallePedido = @Id", new { Id = idDetalle });
    }

    public async Task<DetallePedido> AgregarAsync(DetallePedido detalle)
    {
        using var connection = _connectionFactory.CrearConexion();

        const string sql = @"
            INSERT INTO DetallePedido (IdPedido, IdPizza, Cantidad, PrecioUnitario, Observaciones)
            VALUES (@IdPedido, @IdPizza, @Cantidad, @PrecioUnitario, @Observaciones);
            SELECT LAST_INSERT_ID();";

        var nuevoId = await connection.QuerySingleAsync<int>(sql, detalle);
        detalle.IdDetallePedido = nuevoId;
        return detalle;
    }

    public async Task<int> ActualizarAsync(int idDetalle, int cantidad, string? observaciones)
    {
        using var connection = _connectionFactory.CrearConexion();

        const string sql = @"
            UPDATE DetallePedido
            SET Cantidad = @Cantidad, Observaciones = @Observaciones
            WHERE IdDetallePedido = @Id";

        return await connection.ExecuteAsync(sql, new { Cantidad = cantidad, Observaciones = observaciones, Id = idDetalle });
    }

    public async Task<int> EliminarAsync(int idDetalle)
    {
        using var connection = _connectionFactory.CrearConexion();
        return await connection.ExecuteAsync(
            "DELETE FROM DetallePedido WHERE IdDetallePedido = @Id", new { Id = idDetalle });
    }
}