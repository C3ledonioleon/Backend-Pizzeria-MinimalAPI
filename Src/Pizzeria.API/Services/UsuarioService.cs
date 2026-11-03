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

        public async Task<UsuarioRespuestaDTO> RegistrarAsync(
            RegistrarUsuarioDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Apellido) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Telefono) ||
                string.IsNullOrWhiteSpace(dto.Direccion) ||
                string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                throw new ArgumentException(
                    "Todos los campos son obligatorios."
                );
            }

            if (!dto.Email.Contains('@'))
            {
                throw new ArgumentException("El email no es válido.");
            }

            if (dto.Contrasena.Length < 8)
            {
                throw new ArgumentException(
                    "La contraseña debe tener al menos 8 caracteres."
                );
            }

            var email = dto.Email.Trim().ToLowerInvariant();

            var usuarioExistente =
                await _usuarioRepository.ObtenerPorEmailAsync(email);

            if (usuarioExistente != null)
            {
                throw new InvalidOperationException(
                    "Ya existe una cuenta registrada con ese email."
                );
            }

            // Generamos el hash de la contraseña.
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
            // El registro público siempre crea un Cliente.
            var usuario = new Usuario(
                email,
                passwordHash,
                RolUsuario.Cliente
            );

            var cliente = new Cliente(
                dto.Nombre.Trim(),
                dto.Apellido.Trim(),
                email,
                dto.Telefono.Trim(),
                dto.Direccion.Trim()
            );

            var usuarioCreado =
                await _usuarioRepository.RegistrarClienteAsync(
                    usuario,
                    cliente
                );

            return ConvertirRespuesta(usuarioCreado);
        }

        public async Task<UsuarioRespuestaDTO?> LoginAsync(
            LoginDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                return null;
            }

            var email = dto.Email.Trim().ToLowerInvariant();

            var usuario =
                await _usuarioRepository.ObtenerPorEmailAsync(email);

            if (usuario == null)
            {
                return null;
            }

            // Comparamos la contraseña ingresada con el hash guardado.
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

        private static UsuarioRespuestaDTO ConvertirRespuesta(
            Usuario usuario)
        {
            return new UsuarioRespuestaDTO
            {
                IdUsuario = usuario.IdUsuario,
                IdCliente = usuario.IdCliente,
                Email = usuario.Email,
                Rol = usuario.Rol
            };
        }
    }
}
