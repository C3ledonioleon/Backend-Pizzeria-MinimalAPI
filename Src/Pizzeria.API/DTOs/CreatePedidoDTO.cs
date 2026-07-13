public class CreatePedidoDto
{
    public int ClienteId { get; set; }
    public List<int> PizzaIds { get; set; } = new();  // IDs de las pizzas que se quieren pedir
}