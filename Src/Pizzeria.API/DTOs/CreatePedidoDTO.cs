namespace Pizzeria.API.DTOs;

public class CreatePedidoDto
{
    public int IdCliente { get; set; }
    public int IdSucursal { get; set; }
    public List<DetallePedidoDto> Detalles { get; set; } = new();
}