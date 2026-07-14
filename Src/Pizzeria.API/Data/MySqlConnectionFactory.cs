using System.Data;
using MySqlConnector;

namespace Pizzeria.API.Data;

public class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(IConfiguration configuration)
    {
        var candidates = new[] { "6to_Pizzeria", "DefaultConnection", "PizzeriaDB" };
        string? found = null;
        foreach (var name in candidates)
        {
            var cs = configuration.GetConnectionString(name);
            if (!string.IsNullOrWhiteSpace(cs))
            {
                found = cs;
                break;
            }
        }

        _connectionString = found ?? throw new InvalidOperationException($"No se encontró ninguna cadena de conexión en appsettings.json. Busqué: {string.Join(", ", candidates)}. Añade una entrada bajo 'ConnectionStrings'.");
    }

    public IDbConnection CrearConexion() => new MySqlConnection(_connectionString);
}