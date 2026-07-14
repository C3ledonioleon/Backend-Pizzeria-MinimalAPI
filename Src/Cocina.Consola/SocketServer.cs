using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Cocina.Consola;

public class SocketServer
{
    private const int Puerto = 6000;
    private TcpListener? _listener;

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
                Console.WriteLine($"[Cocina] Error al procesar conexión: {ex.Message}");
            }
        }
    }

    private async Task ProcesarPedidoAsync(TcpClient cliente)
    {
        using var stream = cliente.GetStream();
        var buffer = new byte[4096];
        var bytesLeidos = await stream.ReadAsync(buffer);

        var mensaje = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);
        Console.WriteLine($"[Cocina] Pedido recibido: {mensaje}");

        try
        {
            using var doc = JsonDocument.Parse(mensaje);
            var idPedido = doc.RootElement.GetProperty("IdPedido").GetInt32();

            Console.WriteLine($"[Cocina] Preparando pedido #{idPedido}...");
            await Task.Delay(2000); // simula tiempo de preparación
            Console.WriteLine($"[Cocina] Pedido #{idPedido} listo. Pasa a reparto.");

            // Acá, más adelante, se podría avisar a Reparto.Consola con el mismo patrón.
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[Cocina] Error al leer el pedido: {ex.Message}");
        }
    }
}