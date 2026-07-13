using System;

public class PedidoResponseDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public List<PizzaDto> Pizzas { get; set; } = new();
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;  // Nombre del enum como string
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}