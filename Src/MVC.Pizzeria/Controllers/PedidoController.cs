using Microsoft.AspNetCore.Mvc;
using MVC.Pizzeria.Models;
using MVC.Pizzeria.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using MVC.Pizzeria.Enums;

namespace MVC.Pizzeria.Controllers;

public class PedidoController : Controller
{
private readonly ApiService _apiService;

public PedidoController(ApiService apiService)
{
    _apiService = apiService;
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
    catch (HttpRequestException)
    {
        TempData["ErrorAdmin"] = "No se pudo actualizar el estado del pedido.";
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

    return Redirect("/Home/Index#menu");
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
        var cliente = await _apiService.GetOptionalAsync<ClienteCreadoResponse>(
            "api/clientes/yo");

        if (cliente is null)
        {
            cliente = await _apiService.PostAsync<CrearClienteRequest, ClienteCreadoResponse>(
                "api/clientes/",
                new CrearClienteRequest
                {
                    Nombre = datos.Nombre ?? string.Empty,
                    Apellido = datos.Apellido ?? string.Empty,
                    Telefono = datos.Telefono ?? string.Empty,
                    Direccion = datos.Direccion ?? string.Empty,
                    Email = datos.Email ?? string.Empty
                });
        }

        if (cliente is null)
        {
            ModelState.AddModelError(string.Empty, "No se pudo registrar al cliente. Intenta nuevamente.");
            return View(datos);
        }

        var pedidoCreado = await _apiService.PostAsync<CrearPedidoRequest, PedidoCreadoResponse>(
            "api/pedidos/",
            new CrearPedidoRequest
            {
                IdCliente = cliente.IdCliente,
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
    catch (HttpRequestException)
    {
        ModelState.AddModelError(string.Empty, "No fue posible conectar con el servicio de pedidos. Intenta nuevamente.");
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
