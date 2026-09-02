using Microsoft.AspNetCore.Mvc;
using MVC.Pizzeria.Models;
using MVC.Pizzeria.Services;
using System.Text.Json;

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
    return Agregar(idPizza);
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
