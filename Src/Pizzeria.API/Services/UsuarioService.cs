
using Pizzeria.API.DTOs;
using Pizzeria.API.Enums;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;
using Pizzeria.API.Services.IServices;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Pizzeria.API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
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
            string password = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

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

            var (contrasenaCorrecta, necesitaMigracion) =
                VerificarContrasena(usuario.Password, dto.Contrasena);

            if (!contrasenaCorrecta)
            {
                return null;
            }

            if (necesitaMigracion)
            {
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
                await _usuarioRepository.ActualizarContrasenaAsync(
                    usuario.IdUsuario,
                    usuario.Password);
            }

            // Generar respuesta con JWT
            var respuesta = ConvertirRespuesta(usuario);

            respuesta.Token = GenerarToken(usuario);

            return respuesta;
        }

        private static (bool EsValida, bool NecesitaMigracion) VerificarContrasena(
            string almacenada,
            string ingresada)
        {
            if (Regex.IsMatch(
                    almacenada,
                    @"^\$2[aby]\$\d{2}\$[./A-Za-z0-9]{53}$",
                    RegexOptions.CultureInvariant))
            {
                return (BCrypt.Net.BCrypt.Verify(ingresada, almacenada), false);
            }

            if (almacenada.StartsWith("$2", StringComparison.Ordinal))
                return (false, false);

            var bytesIngresados = Encoding.UTF8.GetBytes(ingresada);
            var bytesAlmacenados = Encoding.UTF8.GetBytes(almacenada);
            if (CryptographicOperations.FixedTimeEquals(bytesIngresados, bytesAlmacenados))
                return (true, true);

            if (!Regex.IsMatch(almacenada, @"\A[0-9a-fA-F]{64}\z"))
                return (false, false);

            var hashIngresado = SHA256.HashData(bytesIngresados);
            var hashAlmacenado = Convert.FromHexString(almacenada);
            return (CryptographicOperations.FixedTimeEquals(hashIngresado, hashAlmacenado), true);
        }

        // GENERAR TOKEN JWT
        private string GenerarToken(Usuario usuario)
        {
            var key = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(key) ||
                Encoding.UTF8.GetByteCount(key) < 32)
            {
                throw new InvalidOperationException(
                    "La clave JWT debe tener al menos 32 bytes."
                );
            }

            var secretKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            );

            var credentials = new SigningCredentials(
                secretKey,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,usuario.IdUsuario.ToString() ),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role,usuario.Rol.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _configuration.GetValue<int>("Jwt:ExpirationMinutes")
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
