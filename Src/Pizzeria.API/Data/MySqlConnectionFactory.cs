using System.Data;
using MySqlConnector;

namespace Pizzeria.API.Data;

public class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PizzeriaDB")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'PizzeriaDB' en appsettings.json");
    }

    public IDbConnection CrearConexion() => new MySqlConnection(_connectionString);
}