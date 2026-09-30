namespace MVC.Pizzeria.Models;

public class CrearClienteRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}

public class ConfirmarPedidoViewModel
{
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Email { get; set; }
}

public class CrearPedidoRequest
{
    public int IdCliente { get; set; }
    public List<CrearDetallePedidoRequest> Detalles { get; set; } = new();
}

public class CrearDetallePedidoRequest
{
    public int IdPizza { get; set; }
    public int Cantidad { get; set; }
    public string Observaciones { get; set; } = string.Empty;
}

public class ClienteCreadoResponse
{
    public int IdCliente { get; set; }
}

public class PedidoCreadoResponse
{
    public int IdPedido { get; set; }
}

