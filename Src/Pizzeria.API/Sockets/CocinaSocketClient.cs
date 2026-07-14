using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Pizzeria.API.Models;

namespace Pizzeria.API.Sockets;

public class CocinaSocketClient
{
    private const string Host = "127.0.0.1";
    private const int Puerto = 6000;

    public async Task NotificarNuevoPedidoAsync(Pedido pedido)
    {
        using var cliente = new TcpClient();

        // Timeout de conexión: si la cocina no responde en 3 segundos, se considera caída.
        var conexionTask = cliente.ConnectAsync(Host, Puerto);
        var completado = await Task.WhenAny(conexionTask, Task.Delay(3000));

        if (completado != conexionTask || !cliente.Connected)
            throw new Exception("No se pudo conectar con el servicio de Cocina (posible caída del servicio).");

        var mensaje = JsonSerializer.Serialize(new
        {
            Tipo = "NuevoPedido",
            pedido.IdPedido,
            pedido.IdCliente,
            pedido.Total,
            Detalles = pedido.Detalles.Select(d => new { d.IdPizza, d.Cantidad, d.Observaciones })
        });

        var bytes = Encoding.UTF8.GetBytes(mensaje);

        using var stream = cliente.GetStream();
        await stream.WriteAsync(bytes);

        Console.WriteLine($"[Socket] Pedido #{pedido.IdPedido} enviado a Cocina.");
    }
}