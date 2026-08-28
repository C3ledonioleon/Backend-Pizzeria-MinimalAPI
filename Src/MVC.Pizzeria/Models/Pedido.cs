
using MVC.Pizzeria.Enums;

namespace MVC.Pizzeria.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public DateTime FechaHora { get; set; }
        public EstadoPedido Estado { get; set; }
        public int IdCliente { get; set; }
        public decimal Total { get; set; }
        public List<DetallePedido> Detalles { get; set; } = new();
    }
}