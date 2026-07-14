using Pizzeria.API.DTOs;

namespace Pizzeria.API.Validators;

public static class EmpleadoValidator
{
    public static List<string> Validar(CreateEmpleadoDto dto)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Nombre))
            errores.Add("El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Apellido))
            errores.Add("El apellido es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.DNI))
            errores.Add("El DNI es obligatorio.");

        if (dto.IdsSucursales is null || dto.IdsSucursales.Count == 0)
            errores.Add("El empleado debe estar asignado al menos a una sucursal.");

        return errores;
    }
}