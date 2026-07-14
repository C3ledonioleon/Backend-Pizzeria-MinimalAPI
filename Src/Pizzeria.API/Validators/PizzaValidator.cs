using Pizzeria.API.DTOs;

namespace Pizzeria.API.Validators;

public static class PizzaValidator
{
    public static List<string> Validar(CreatePizzaDto dto)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Nombre))
            errores.Add("El nombre de la pizza es obligatorio.");

        if (dto.Precio <= 0)
            errores.Add("El precio debe ser mayor a 0.");

        return errores;
    }
}