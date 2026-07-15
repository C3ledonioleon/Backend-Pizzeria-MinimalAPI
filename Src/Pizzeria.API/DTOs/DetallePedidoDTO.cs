namespace Pizzeria.API.DTOs;

public class DetallePedidoDto
{
    public int IdPizza { get; set; }
    public int Cantidad { get; set; }
    public string? Observaciones { get; set; }
}
public class ActualizarDetalleDto
{
    public int Cantidad { get; set; }
    public string? Observaciones { get; set; }
}