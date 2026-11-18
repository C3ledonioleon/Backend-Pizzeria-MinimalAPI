using System.ComponentModel.DataAnnotations;
using MVC.Pizzeria.Enums;

namespace MVC.Pizzeria.Models;

public class ConfirmarPedidoViewModel
{
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }

    [Required(ErrorMessage = "Seleccioná un medio de pago.")]
    [EnumDataType(typeof(MedioPago), ErrorMessage = "El medio de pago seleccionado no es válido.")]
    public MedioPago? MedioPago { get; set; }
}

public class CrearPedidoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public int MedioPago { get; set; }
    public List<CrearDetallePedidoRequest> Detalles { get; set; } = new();
}

public class CrearDetallePedidoRequest
{
    public int IdPizza { get; set; }
    public int Cantidad { get; set; }
    public string Observaciones { get; set; } = string.Empty;
}

public class PedidoCreadoResponse
{
    public int IdPedido { get; set; }
}

