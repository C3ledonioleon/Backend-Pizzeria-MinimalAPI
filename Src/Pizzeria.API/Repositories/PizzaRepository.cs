using Dapper;
using Pizzeria.API.Data;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Repositories;

public class PizzaRepository : IPizzaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PizzaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Pizza>> ObtenerTodasAsync()
    {
        using var connection = _connectionFactory.CrearConexion();
        var filas = await connection.QueryAsync<PizzaRow>("SELECT * FROM Pizza");
        return filas.Select(MapearAPizza).ToList();
    }

    public async Task<Pizza?> ObtenerPorIdAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();
        var fila = await connection.QuerySingleOrDefaultAsync<PizzaRow>(
            "SELECT * FROM Pizza WHERE IdPizza = @Id", new { Id = id });

        return fila is null ? null : MapearAPizza(fila);
    }

    public async Task<Pizza> CrearAsync(Pizza pizza)
    {
        using var connection = _connectionFactory.CrearConexion();

        const string sql = @"
            INSERT INTO Pizza (Nombre, Precio, Descripcion, Ingredientes)
            VALUES (@Nombre, @Precio, @Descripcion, @Ingredientes);
            SELECT LAST_INSERT_ID();";

        var nuevoId = await connection.QuerySingleAsync<int>(sql, new
        {
            pizza.Nombre,
            pizza.Precio,
            pizza.Descripcion,
            Ingredientes = string.Join(",", pizza.Ingredientes)
        });

        pizza.IdPizza = nuevoId;
        return pizza;
    }

    public async Task<int> ActualizarAsync(int id, Pizza pizza)
    {
        using var connection = _connectionFactory.CrearConexion();

        const string sql = @"
            UPDATE Pizza
            SET Nombre = @Nombre, Precio = @Precio,
                Descripcion = @Descripcion, Ingredientes = @Ingredientes
            WHERE IdPizza = @Id";

        return await connection.ExecuteAsync(sql, new
        {
            pizza.Nombre,
            pizza.Precio,
            pizza.Descripcion,
            Ingredientes = string.Join(",", pizza.Ingredientes),
            Id = id
        });
    }

    public async Task<int> EliminarAsync(int id)
    {
        using var connection = _connectionFactory.CrearConexion();
        return await connection.ExecuteAsync(
            "DELETE FROM Pizza WHERE IdPizza = @Id", new { Id = id });
    }

    private class PizzaRow
    {
        public int IdPizza { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }
        public string? Ingredientes { get; set; }
    }

    private static Pizza MapearAPizza(PizzaRow fila) => new()
    {
        IdPizza = fila.IdPizza,
        Nombre = fila.Nombre,
        Precio = fila.Precio,
        Descripcion = fila.Descripcion,
        Ingredientes = string.IsNullOrEmpty(fila.Ingredientes)
            ? new List<string>()
            : fila.Ingredientes.Split(',').ToList()
    };
}