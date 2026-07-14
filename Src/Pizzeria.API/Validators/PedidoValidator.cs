using Pizzeria.API.DTOs;

namespace Pizzeria.API.Validators;

public static class PedidoValidator
{
    public static List<string> Validar(CreatePedidoDto dto)
    {
        var errores = new List<string>();

        if (dto.IdCliente <= 0)
            errores.Add("Debe indicar un cliente válido.");

        if (dto.IdSucursal <= 0)
            errores.Add("Debe indicar una sucursal válida.");

        if (dto.Detalles is null || dto.Detalles.Count == 0)
            errores.Add("El pedido debe tener al menos una pizza.");
        else
        {
            foreach (var detalle in dto.Detalles)
            {
                if (detalle.Cantidad <= 0)
                    errores.Add($"La cantidad para la pizza {detalle.IdPizza} debe ser mayor a 0.");
            }
        }

        return errores;
    }
}