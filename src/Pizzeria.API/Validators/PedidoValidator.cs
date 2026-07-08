using System.ComponentModel.DataAnnotations;

public class PedidoCreateDto
{
    [Required(ErrorMessage = "El ClienteId es obligatorio")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "Debe incluir al menos una pizza")]
    [MinLength(1, ErrorMessage = "El pedido debe tener al menos una pizza")]
    public List<PizzaPedidoCreateDto> Pizzas { get; set; } = new();

    [StringLength(200, ErrorMessage = "Las observaciones no pueden superar los 200 caracteres")]
    public string? Observaciones { get; set; }
}