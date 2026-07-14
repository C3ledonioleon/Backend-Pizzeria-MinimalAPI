namespace Pizzeria.API.Models;

public class Sucursal
{
    public int IdSucursal { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;

    public Sucursal() { }

    public Sucursal(string nombre, string direccion, string telefono)
    {
        Nombre = nombre;
        Direccion = direccion;
        Telefono = telefono;
    }
}