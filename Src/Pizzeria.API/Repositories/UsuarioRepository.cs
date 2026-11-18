
using Dapper;
using Pizzeria.API.Data;
using Pizzeria.API.Models;

namespace Pizzeria.API.Repositories.IRepositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsuarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int idUsuario)
        {
            using var connection = _connectionFactory.CrearConexion();

            const string sql = @"
                SELECT
                    u.IdUsuario,
                    u.Username,
                    u.Email,
                    u.Password,
                    u.Rol,
                    c.IdCliente
                FROM Usuario u
                LEFT JOIN Cliente c ON c.IdUsuario = u.IdUsuario
                WHERE u.IdUsuario = @IdUsuario";

            return await connection.QuerySingleOrDefaultAsync<Usuario>(
                sql,
                new { IdUsuario = idUsuario }
            );
        }

        public async Task<Usuario?> ObtenerPorUsuarioOEmailAsync(string usuarioOEmail)
        {
            using var connection = _connectionFactory.CrearConexion();

            const string sql = @"
                SELECT
                    u.IdUsuario,
                    u.Username,
                    u.Email,
                    u.Password,
                    u.Rol,
                    c.IdCliente
                FROM Usuario u
                LEFT JOIN Cliente c ON c.IdUsuario = u.IdUsuario
                WHERE LOWER(u.Username) = LOWER(@UsuarioOEmail)
                OR LOWER(u.Email) = LOWER(@UsuarioOEmail)
                LIMIT 1";

            return await connection.QuerySingleOrDefaultAsync<Usuario>(
                sql,
                new { UsuarioOEmail = usuarioOEmail.Trim() }
            );
        }

        public async Task<Usuario> RegistrarAsync(Usuario usuario)
        {
            using var connection = _connectionFactory.CrearConexion();

            const string sql = @"
                INSERT INTO Usuario (Username, Email, Password, Rol)
                VALUES (@Username, @Email, @Password, @Rol);

                SELECT LAST_INSERT_ID();";

            var idUsuario = await connection.QuerySingleAsync<int>(
                sql,
                new
                {
                    usuario.Username,
                    usuario.Email,
                    usuario.Password,
                    Rol = (int)usuario.Rol
                }
            );

            usuario.IdUsuario = idUsuario;

            return usuario;
        }

        public async Task ActualizarContrasenaAsync(int idUsuario, string passwordHash)
        {
            using var connection = _connectionFactory.CrearConexion();

            await connection.ExecuteAsync(
                "UPDATE Usuario SET Password = @Password WHERE IdUsuario = @IdUsuario",
                new
                {
                    Password = passwordHash,
                    IdUsuario = idUsuario
                }
            );
        }
    }
}
