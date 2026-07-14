namespace Pizzeria.API.Models;

public class EmpleadoSucursal
{
    public int IdEmpleado { get; set; }
    public int IdSucursal { get; set; }

    public EmpleadoSucursal() { }

    public EmpleadoSucursal(int idEmpleado, int idSucursal)
    {
        IdEmpleado = idEmpleado;
        IdSucursal = idSucursal;
    }
}