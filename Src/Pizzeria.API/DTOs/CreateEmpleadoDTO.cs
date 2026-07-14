using Pizzeria.API.Enums;

namespace Pizzeria.API.DTOs;

public class CreateEmpleadoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public RolEmpleado Rol { get; set; }
    public string DNI { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public List<int> IdsSucursales { get; set; } = new();
}