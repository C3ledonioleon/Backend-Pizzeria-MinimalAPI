
using Pizzeria.API.Enums;

namespace Pizzeria.API.DTOs;

public class CreatePedidoDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public MedioPago MedioPago { get; set; }

    public List<DetallePedidoDto> Detalles { get; set; } = new();
}
