using Dapper;
using Pizzeria.API.Data;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsuarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            using var connection = _connectionFactory.CrearConexion();

            const string sql = @"
                SELECT
                    u.IdUsuario,
                    u.Email,
                    u.Password,
                    u.Rol,
                    c.IdCliente
                FROM Usuario u
                LEFT JOIN Cliente c ON c.IdUsuario = u.IdUsuario
                WHERE u.Email = @Email";

            return await connection.QuerySingleOrDefaultAsync<Usuario>(
                sql,
                new { Email = email }
            );
        }

        public async Task<Usuario> RegistrarClienteAsync(
            Usuario usuario,
            Cliente cliente)
        {
            using var connection = _connectionFactory.CrearConexion();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                const string sqlUsuario = @"
                    INSERT INTO Usuario (Email, Password, Rol)
                    VALUES (@Email, @Password, @Rol);

                    SELECT LAST_INSERT_ID();";

                var idUsuario = await connection.QuerySingleAsync<int>(
                    sqlUsuario,
                    new
                    {
                        usuario.Email,
                        usuario.Password,
                        Rol = (int)usuario.Rol
                    },
                    transaction
                );

                const string sqlCliente = @"
                    INSERT INTO Cliente
                    (IdUsuario, Nombre, Apellido, Email, Telefono, Direccion)
                    VALUES
                    (@IdUsuario, @Nombre, @Apellido, @Email, @Telefono, @Direccion);

                    SELECT LAST_INSERT_ID();";

                var idCliente = await connection.QuerySingleAsync<int>(
                    sqlCliente,
                    new
                    {
                        IdUsuario = idUsuario,
                        cliente.Nombre,
                        cliente.Apellido,
                        cliente.Email,
                        cliente.Telefono,
                        cliente.Direccion
                    },
                    transaction
                );

                transaction.Commit();

                usuario.IdUsuario = idUsuario;
                usuario.IdCliente = idCliente;

                return usuario;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
