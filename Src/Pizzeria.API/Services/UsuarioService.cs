
using Pizzeria.API.DTOs;
using Pizzeria.API.Enums;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;
using Pizzeria.API.Services.IServices;

namespace Pizzeria.API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // REGISTRO
        public async Task<UsuarioRespuestaDTO> RegistrarAsync(
            RegistrarUsuarioDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                throw new ArgumentException(
                    "Todos los campos son obligatorios."
                );
            }

            string username = dto.Username.Trim();
            string email = dto.Email.Trim().ToLowerInvariant();

            if (!email.Contains('@'))
            {
                throw new ArgumentException("El email no es válido.");
            }

            if (dto.Contrasena.Length < 8)
            {
                throw new ArgumentException(
                    "La contraseña debe tener al menos 8 caracteres."
                );
            }

            // Comprobar si ya existe el nombre de usuario
            var usuarioExistente =
                await _usuarioRepository.ObtenerPorUsuarioOEmailAsync(username);

            if (usuarioExistente != null)
            {
                throw new InvalidOperationException(
                    "Ese nombre de usuario ya está registrado."
                );
            }

            // Comprobar si ya existe el email
            var emailExistente =
                await _usuarioRepository.ObtenerPorUsuarioOEmailAsync(email);

            if (emailExistente != null)
            {
                throw new InvalidOperationException(
                    "Ya existe una cuenta registrada con ese email."
                );
            }

            // Proteger la contraseña
            string password =
                BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            // Crear usuario con rol Cliente
            var usuario = new Usuario(
                username,
                email,
                password,
                RolUsuario.Cliente
            );

            var usuarioCreado =
                await _usuarioRepository.RegistrarAsync(usuario);

            return ConvertirRespuesta(usuarioCreado);
        }

        // INICIO DE SESIÓN
        public async Task<UsuarioRespuestaDTO?> LoginAsync(LoginDTO dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.UsuarioOEmail) ||
                string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                return null;
            }

            string identificador = dto.UsuarioOEmail.Trim();

            var usuario =
                await _usuarioRepository.ObtenerPorUsuarioOEmailAsync(
                    identificador
                );

            if (usuario == null)
            {
                return null;
            }

            bool contrasenaCorrecta = BCrypt.Net.BCrypt.Verify(
                dto.Contrasena,
                usuario.Password
            );

            if (!contrasenaCorrecta)
            {
                return null;
            }

            return ConvertirRespuesta(usuario);
        }

        // CONVERTIR RESPUESTA
        private static UsuarioRespuestaDTO ConvertirRespuesta(
            Usuario usuario)
        {
            return new UsuarioRespuestaDTO
            {
                IdUsuario = usuario.IdUsuario,
                IdCliente = usuario.IdCliente, 
                Username = usuario.Username,
                Email = usuario.Email,
                Rol = usuario.Rol
            };
        }
    }
}
