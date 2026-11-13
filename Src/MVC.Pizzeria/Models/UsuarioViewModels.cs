public class UsuarioRespuestaMVC
{
    public int IdUsuario { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int Rol { get; set; }

    public string Token { get; set; } = string.Empty;
}