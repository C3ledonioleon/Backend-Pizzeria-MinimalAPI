using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Reparto.Consola;

public class SocketServer
{
    private const int Puerto = 6001;
    private TcpListener? _listener;

    public async Task IniciarAsync()
    {
        _listener = new TcpListener(IPAddress.Any, Puerto);
        _listener.Start();
        Console.WriteLine($"[Reparto] Escuchando en el puerto {Puerto}...");

        while (true)
        {
            try
            {
                using var cliente = await _listener.AcceptTcpClientAsync();
                await ProcesarAsync(cliente);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Reparto] Error: {ex.Message}");
            }
        }
    }

    private async Task ProcesarAsync(TcpClient cliente)
    {
        using var stream = cliente.GetStream();
        var buffer = new byte[4096];
        var bytesLeidos = await stream.ReadAsync(buffer);

        var mensaje = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);
        Console.WriteLine($"[Reparto] Mensaje recibido: {mensaje}");

        try
        {
            using var doc = JsonDocument.Parse(mensaje);
            var idPedido = doc.RootElement.GetProperty("IdPedido").GetInt32();

            Console.WriteLine($"[Reparto] Saliendo a entregar pedido #{idPedido}...");
            await Task.Delay(2000);
            Console.WriteLine($"[Reparto] Pedido #{idPedido} entregado.");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[Reparto] Error al leer el mensaje: {ex.Message}");
        }
    }
}