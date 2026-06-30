public class Pedido
{
    public int Id { get; set; }
    public Cliente Cliente { get; set; }
    public List<Pizza> Items { get; set; } = new List<Pizza>();
    
    // Propiedad obligatoria para gestionar los 4 estados requeridos
    public EstadoPedido Estado { get; set; } = EstadoPedido.EsperaDeConfirmacion;
    
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}