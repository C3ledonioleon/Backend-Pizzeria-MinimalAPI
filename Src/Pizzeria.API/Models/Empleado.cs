using Pizzeria.API.Enums;

namespace Pizzeria.API.Models;

public class Empleado
{
    public int IdEmpleado { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public RolEmpleado Rol { get; set; }
    public string DNI { get; set; } = string.Empty;
    public List<int> IdsSucursales { get; set; } = new();
    public string Telefono { get; set; } = string.Empty;

    public Empleado() { }

    public Empleado(string nombre, string apellido, RolEmpleado rol, string dni, string telefono)
    {
        Nombre = nombre;
        Apellido = apellido;
        Rol = rol;
        DNI = dni;
        Telefono = telefono;
    }
}