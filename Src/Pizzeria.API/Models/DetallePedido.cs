using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pizzeria.API.Models
{
    public class DetallePedido
    {
        public int IdDetallePedido { get; set; }
        public int IdPedido { get; set; }
        public int IdPizza { get; set; }        // antes decía IdProducto
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? Observaciones { get; set; }     // ej: "sin aceitunas"

    public DetallePedido() { }

        public DetallePedido(int idPedido, int idPizza, int cantidad, decimal precioUnitario, string? observaciones = null)
        {
            IdPedido = idPedido;
            IdPizza = idPizza;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            Observaciones = observaciones;
        }
    }
}