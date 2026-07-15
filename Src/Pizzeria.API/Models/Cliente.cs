namespace Pizzeria.API.Models;

public class Cliente
{
    public int IdCliente { get; set; }
    public string Nombre { get; set; } 
    public string Apellido { get; set; } 
    public string Email { get; set; } 
    public string Telefono { get; set; } 
    public string Direccion { get; set; } 
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