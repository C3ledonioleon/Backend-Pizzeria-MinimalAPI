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

public async Task<IActionResult> Index()
{
    var carritoJson = HttpContext.Session.GetString("Carrito");

    if (string.IsNullOrEmpty(carritoJson))
    {
        return View(new List<Pizza>());
    }

    var idsPizza = JsonSerializer.Deserialize<List<int>>(carritoJson)
                   ?? new List<int>();

    var pizzas = await _apiService.GetAsync<List<Pizza>>("api/pizzas/");

    var pizzasCarrito = pizzas?
        .Where(p => idsPizza.Contains(p.IdPizza))
        .ToList()
        ?? new List<Pizza>();

    return View(pizzasCarrito);
}


}
