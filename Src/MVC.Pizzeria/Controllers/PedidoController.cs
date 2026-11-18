using Microsoft.AspNetCore.Mvc;
using MVC.Pizzeria.Models;
using MVC.Pizzeria.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using MVC.Pizzeria.Enums;
using System.Net;

namespace MVC.Pizzeria.Controllers;

public class PedidoController : Controller
{
private readonly ApiService _apiService;
private readonly ILogger<PedidoController> _logger;

public PedidoController(ApiService apiService, ILogger<PedidoController> logger)
{
    _apiService = apiService;
    _logger = logger;
}

// Mostrar el pedido
public async Task<IActionResult> Index()
{
    var pedido = ObtenerPedidoDeSesion();
    var pizzas = await _apiService.GetAsync<List<Pizza>>("api/pizzas/")
                 ?? new List<Pizza>();

    foreach (var detalle in pedido.Detalles)
    {
        var pizza = pizzas.FirstOrDefault(p => p.IdPizza == detalle.IdPizza);

        if (pizza == null)
        {
            continue;
        }

        detalle.NombrePizza = pizza.Nombre;
        detalle.PrecioUnitario = pizza.Precio;
    }

    pedido.Total = pedido.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

    return View(pedido);
}

[Authorize(Roles = "Cliente")]
public async Task<IActionResult> Historia()
{
    var pedidos = await _apiService.GetAsync<List<Pedido>>("api/pedidos/mis-pedidos")
                  ?? new List<Pedido>();
    var pizzas = await _apiService.GetAsync<List<Pizza>>("api/pizzas/")
                 ?? new List<Pizza>();

    foreach (var detalle in pedidos.SelectMany(p => p.Detalles))
    {
        var pizza = pizzas.FirstOrDefault(p => p.IdPizza == detalle.IdPizza);
        if (pizza is null)
            continue;

        detalle.NombrePizza = pizza.Nombre;
    }

    return View(pedidos);
}

[Authorize(Roles = "Administrador")]
public async Task<IActionResult> Administrar()
{
    var pedidos = await _apiService.GetAsync<List<Pedido>>("api/pedidos/")
                  ?? new List<Pedido>();
    var pizzas = await _apiService.GetAsync<List<Pizza>>("api/pizzas/")
                 ?? new List<Pizza>();

    foreach (var detalle in pedidos.SelectMany(p => p.Detalles))
    {
        var pizza = pizzas.FirstOrDefault(p => p.IdPizza == detalle.IdPizza);
        if (pizza is not null)
            detalle.NombrePizza = pizza.Nombre;
    }

    return View(pedidos);
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Administrador")]
public async Task<IActionResult> CambiarEstado(int idPedido, EstadoPedido estado)
{
    if (!Enum.IsDefined(estado))
    {
        TempData["ErrorAdmin"] = "El estado seleccionado no es válido.";
        return RedirectToAction(nameof(Administrar));
    }

    try
    {
        await _apiService.PutAsync(
            $"api/pedidos/{idPedido}/estado?estado={(int)estado}");
        TempData["MensajeAdmin"] = $"Se actualizó el estado del pedido #{idPedido}.";
    }
    catch (HttpRequestException ex)
    {
        TempData["ErrorAdmin"] = ex.StatusCode switch
        {
            HttpStatusCode.Conflict => "Transición inválida: el pedido no puede retroceder o pasar a un estado incompatible.",
            HttpStatusCode.BadRequest => "El estado seleccionado no es válido.",
            HttpStatusCode.NotFound => "No se encontró el pedido solicitado.",
            _ => "No se pudo actualizar el estado del pedido."
        };
    }

    return RedirectToAction(nameof(Administrar));
}


// Agregar una pizza al pedido
[HttpPost]
public IActionResult Agregar(int idPizza)
{
    var pedido = ObtenerPedidoDeSesion();
    var detalle = pedido.Detalles
        .FirstOrDefault(d => d.IdPizza == idPizza);


    if (detalle != null)
    {
        detalle.Cantidad++;
    }
    else
    {
        pedido.Detalles.Add(new DetallePedido
        {
            IdPizza = idPizza,
            Cantidad = 1
        });
    }

    GuardarPedidoEnSesion(pedido);

    TempData["PizzaAgregada"] = "¡Pizza agregada a tu pedido!";

    return Redirect($"/Home/Index#pizza-{idPizza}");
}

[HttpPost]
public IActionResult Incrementar(int idPizza)
{
    var pedido = ObtenerPedidoDeSesion();
    var detalle = pedido.Detalles.FirstOrDefault(d => d.IdPizza == idPizza);

    if (detalle != null)
    {
        detalle.Cantidad++;
        GuardarPedidoEnSesion(pedido);
    }

    return RedirectToAction(nameof(Index));
}

[HttpPost]
public IActionResult Disminuir(int idPizza)
{
    var pedido = ObtenerPedidoDeSesion();
    var detalle = pedido.Detalles.FirstOrDefault(d => d.IdPizza == idPizza);

    if (detalle != null)
    {
        detalle.Cantidad--;

        if (detalle.Cantidad <= 0)
        {
            pedido.Detalles.Remove(detalle);
        }
    }

    GuardarPedidoEnSesion(pedido);
    return RedirectToAction(nameof(Index));
}

[HttpPost]
public IActionResult Cancelar()
{
    HttpContext.Session.Remove("Pedido");
    return Redirect("/Home/Index#menu");
}

[HttpGet]
[Authorize]
public IActionResult Confirmar()
{
    var pedido = ObtenerPedidoDeSesion();
    if (!pedido.Detalles.Any())
    {
        return RedirectToAction(nameof(Index));
    }

    return View(new ConfirmarPedidoViewModel());
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public async Task<IActionResult> Confirmar(ConfirmarPedidoViewModel datos)
{
    var pedido = ObtenerPedidoDeSesion();
    if (!pedido.Detalles.Any())
    {
        return RedirectToAction(nameof(Index));
    }

    if (!ModelState.IsValid)
    {
        return View(datos);
    }

    try
    {
        var pedidoCreado = await _apiService.PostAsync<CrearPedidoRequest, PedidoCreadoResponse>(
            "api/pedidos/",
            new CrearPedidoRequest
            {
                Nombre = datos.Nombre ?? string.Empty,
                Apellido = datos.Apellido ?? string.Empty,
                Telefono = datos.Telefono ?? string.Empty,
                Direccion = datos.Direccion ?? string.Empty,
                MedioPago = (int)datos.MedioPago!.Value,
                Detalles = pedido.Detalles.Select(detalle => new CrearDetallePedidoRequest
                {
                    IdPizza = detalle.IdPizza,
                    Cantidad = detalle.Cantidad,
                    Observaciones = detalle.Observaciones ?? string.Empty
                }).ToList()
            });

        if (pedidoCreado is null)
        {
            ModelState.AddModelError(string.Empty, "No se pudo crear el pedido. Intenta nuevamente.");
            return View(datos);
        }

        HttpContext.Session.Remove("Pedido");
        TempData["PedidoConfirmado"] = $"¡Pedido #{pedidoCreado.IdPedido} confirmado! Pronto nos pondremos en contacto contigo.";
        return RedirectToAction(nameof(Index));
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "No se pudo confirmar el pedido para el usuario {Username}", User.Identity?.Name);

        var mensaje = ex.StatusCode switch
        {
            HttpStatusCode.InternalServerError =>
                "La API no pudo guardar el pedido. Si la base ya existía, ejecutá Scripts/MigracionMedioPagoPedido.sql y volvé a intentar.",
            HttpStatusCode.Unauthorized =>
                "Tu sesión venció. Cerrá sesión e iniciá nuevamente para confirmar el pedido.",
            HttpStatusCode.Forbidden =>
                "Tu cuenta no tiene permisos para confirmar pedidos.",
            HttpStatusCode.BadRequest =>
                "La API rechazó los datos del pedido. Volvé a seleccionar el medio de pago e intentá nuevamente.",
            null =>
                "No fue posible conectar con la API. Verificá que Pizzeria.API esté ejecutándose en localhost:5260.",
            _ =>
                "No se pudo confirmar el pedido. Intentá nuevamente."
        };

        ModelState.AddModelError(string.Empty, mensaje);
        return View(datos);
    }
}

private Pedido ObtenerPedidoDeSesion()
{
    var pedidoJson = HttpContext.Session.GetString("Pedido");

    return string.IsNullOrEmpty(pedidoJson)
        ? new Pedido()
        : JsonSerializer.Deserialize<Pedido>(pedidoJson) ?? new Pedido();
}

private void GuardarPedidoEnSesion(Pedido pedido)
{
    HttpContext.Session.SetString("Pedido", JsonSerializer.Serialize(pedido));
}

}
