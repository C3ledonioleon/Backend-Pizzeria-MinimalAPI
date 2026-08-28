using Microsoft.AspNetCore.Mvc;
using MVC.Pizzeria.Models;
using MVC.Pizzeria.Services;
using System.Text.Json;

namespace MVC.Pizzeria.Controllers;

public class HomeController : Controller
{
private readonly ApiService _apiService;


public HomeController(ApiService apiService)
{
    _apiService = apiService;
}

public async Task<IActionResult> Index()
{
    var pizzas = await _apiService.GetAsync<List<Pizza>>("api/pizzas/");

    return View(pizzas);
}

[HttpPost]
public IActionResult AgregarAlPedido(int idPizza)
{
    var carritoJson = HttpContext.Session.GetString("Carrito");

    List<int> carrito;

    if (string.IsNullOrEmpty(carritoJson))
    {
        carrito = new List<int>();
    }
    else
    {
        carrito = JsonSerializer.Deserialize<List<int>>(carritoJson)
                   ?? new List<int>();
    }

    carrito.Add(idPizza);

    HttpContext.Session.SetString(
        "Carrito",
        JsonSerializer.Serialize(carrito)
    );

    return RedirectToAction("Index");
}

}

