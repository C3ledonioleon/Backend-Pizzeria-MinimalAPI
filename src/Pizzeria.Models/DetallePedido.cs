public class DetallePedido
{
    public int IdPizza { get; set; }
    public Pizza Pizza { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal Subtotal => Pizza.Precio * Cantidad;
}