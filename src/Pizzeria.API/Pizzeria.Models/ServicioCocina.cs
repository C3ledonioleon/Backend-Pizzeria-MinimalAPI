using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ServicioCocina
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "Cocina Principal";
    public bool EstaActiva { get; set; } = true;
    public Queue<Pedido> PedidosEnCola { get; set; } = new();

    public event Action<Pedido>? PedidoListo;

    public void RecibirPedido(Pedido pedido)
    {
        if (!EstaActiva)
            throw new InvalidOperationException("La cocina está fuera de servicio.");

        PedidosEnCola.Enqueue(pedido);
        pedido.ActualizarEstado(EstadoPedido.EnPreparacion);
        Console.WriteLine($"[Cocina] Pedido {pedido.Id} recibido y en preparación.");
    }

    public async Task SimularPreparacionAsync(Pedido pedido)
    {
        await Task.Delay(3000); // Simula tiempo de preparación
        pedido.ActualizarEstado(EstadoPedido.EnViaje);
        PedidoListo?.Invoke(pedido);
        Console.WriteLine($"[Cocina] Pedido {pedido.Id} listo para entrega.");
    }
}