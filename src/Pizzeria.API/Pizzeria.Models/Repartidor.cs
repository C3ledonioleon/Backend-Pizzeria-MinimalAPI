using System.Threading.Tasks;

public class Repartidor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool EstaDisponible { get; set; } = true;

    public Repartidor(string nombre)
    {
        Nombre = nombre;
    }

    public async Task EntregarPedidoAsync(Pedido pedido)
    {
        if (!EstaDisponible)
            throw new InvalidOperationException("Repartidor no disponible.");

        await Task.Delay(4000); // Simula tiempo de entrega
        pedido.ActualizarEstado(EstadoPedido.Entregado);
        Console.WriteLine($"[Repartidor {Nombre}] Pedido {pedido.Id} entregado exitosamente.");
    }
}