using System.Data;

namespace Pizzeria.API.Data;

public interface IDbConnectionFactory
{
    IDbConnection CrearConexion();
}