using Pizzeria.API.Enums;

namespace Pizzeria.API.Models;

public class Pedido
{
    public int IdPedido { get; set; }
    public DateTime FechaHora { get; set; }
    public EstadoPedido Estado { get; set; }
    public int IdCliente { get; set; }
    public decimal Total { get; set; }
    public List<DetallePedido> Detalles { get; set; } = new();

    public Pedido() { }

    public Pedido(int idCliente)
    {
        IdCliente = idCliente;
        FechaHora = DateTime.Now;
        Estado = EstadoPedido.EsperaConfirmacion;
        Detalles = new List<DetallePedido>();
        Total = 0;
    }
}