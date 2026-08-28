using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Pizzeria.Models
{
    public class DetallePedido
    {
        public int IdDetallePedido { get; set; }
        public int IdPedido { get; set; }
        public int IdPizza { get; set; }       
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string Observaciones { get; set; }     // ej: "sin aceitunas"
    }
}