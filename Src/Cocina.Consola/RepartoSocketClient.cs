using System.Net.Sockets;
using System.Text;

namespace Cocina.Consola;

public class RepartoSocketClient
{
    private const string Host = "127.0.0.1";
    private const int Puerto = 6001;

    public async Task NotificarPedidoListoAsync(string mensajeJson)
    {
        using var cliente = new TcpClient();

        var conexionTask = cliente.ConnectAsync(Host, Puerto);
        var completado = await Task.WhenAny(conexionTask, Task.Delay(3000));

        if (completado != conexionTask || !cliente.Connected)
        {
            Console.WriteLine("[Cocina] No se pudo conectar con Reparto (posible caida del servicio).");
            return;
        }

        var bytes = Encoding.UTF8.GetBytes(mensajeJson);
        using var stream = cliente.GetStream();
        await stream.WriteAsync(bytes);
    }
}