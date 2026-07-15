using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Cocina.Consola;

public class SocketServer
{
    private const int Puerto = 6000;
    private TcpListener? _listener;
    private readonly RepartoSocketClient _repartoClient = new();

    public async Task IniciarAsync()
    {
        _listener = new TcpListener(IPAddress.Any, Puerto);
        _listener.Start();
        Console.WriteLine($"[Cocina] Escuchando pedidos en el puerto {Puerto}...");

        while (true)
        {
            try
            {
                using var cliente = await _listener.AcceptTcpClientAsync();
                await ProcesarPedidoAsync(cliente);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cocina] Error al procesar conexion: {ex.Message}");
            }
        }
    }

    private async Task ProcesarPedidoAsync(TcpClient cliente)
    {
        using var stream = cliente.GetStream();
        var buffer = new byte[4096];
        var bytesLeidos = await stream.ReadAsync(buffer);

        var mensaje = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

        try
        {
            using var doc = JsonDocument.Parse(mensaje);
            var root = doc.RootElement;

            var idPedido = root.GetProperty("IdPedido").GetInt32();
            var idCliente = root.GetProperty("IdCliente").GetInt32();
            var total = root.GetProperty("Total").GetDecimal();

            Console.WriteLine();
            Console.WriteLine($"[Cocina] Nuevo pedido #{idPedido} del cliente #{idCliente} - Total: ${total}");

            if (root.TryGetProperty("Detalles", out var detalles))
            {
                foreach (var detalle in detalles.EnumerateArray())
                {
                    var idPizza = detalle.GetProperty("IdPizza").GetInt32();
                    var cantidad = detalle.GetProperty("Cantidad").GetInt32();
                    Console.WriteLine($"  - Pizza #{idPizza} x{cantidad}");
                }
            }

            Console.WriteLine($"[Cocina] Preparando pedido #{idPedido}...");
            await Task.Delay(3000);
            Console.WriteLine($"[Cocina] Pedido #{idPedido} listo. Avisando a Reparto...");

            await _repartoClient.NotificarPedidoListoAsync(mensaje);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[Cocina] Error al leer el pedido: {ex.Message}");
        }
    }
}