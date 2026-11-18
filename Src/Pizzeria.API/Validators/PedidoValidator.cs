using Pizzeria.API.DTOs;
using Pizzeria.API.Enums;

namespace Pizzeria.API.Validators;

public static class PedidoValidator
{
    public static List<string> Validar(CreatePedidoDto dto)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Nombre) ||
            string.IsNullOrWhiteSpace(dto.Apellido) ||
            string.IsNullOrWhiteSpace(dto.Telefono) ||
            string.IsNullOrWhiteSpace(dto.Direccion))
        {
            errores.Add("Debe indicar los datos de entrega del cliente.");
        }

        if (!Enum.IsDefined(dto.MedioPago))
            errores.Add("Debe indicar un medio de pago válido.");

        if (dto.Detalles is null || dto.Detalles.Count == 0)
            errores.Add("El pedido debe tener al menos una pizza.");
        else
        {
            foreach (var detalle in dto.Detalles)
            {
                if (detalle.IdPizza <= 0)
                    errores.Add("Debe indicar una pizza válida.");

                if (detalle.Cantidad <= 0)
                    errores.Add($"La cantidad para la pizza {detalle.IdPizza} debe ser mayor a 0.");
            }
        }

        return errores;
    }
}