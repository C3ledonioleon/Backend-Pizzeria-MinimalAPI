namespace Pizzeria.API.Models;

public class Cliente
{
    public int IdCliente { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;

    public Cliente() { }

    public Cliente(string nombre, string apellido, string email, string telefono, string direccion)
    {
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Telefono = telefono;
        Direccion = direccion;
    }
}