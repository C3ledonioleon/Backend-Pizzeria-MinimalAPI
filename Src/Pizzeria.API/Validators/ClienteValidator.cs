using Pizzeria.API.DTOs;

namespace Pizzeria.API.Validators;

public static class ClienteValidator
{
    public static List<string> Validar(CreateClienteDto dto)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Nombre))
            errores.Add("El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Apellido))
            errores.Add("El apellido es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
            errores.Add("El email no es válido.");

        if (string.IsNullOrWhiteSpace(dto.Telefono))
            errores.Add("El teléfono es obligatorio.");

        return errores;
    }
}