using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MVC.Pizzeria.Models;

namespace MVC.Pizzeria.Controllers;

public class PizzaController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PizzaController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("PizzeriaAPI");

        var pizzas = await client.GetFromJsonAsync<List<Pizza>>(
            "/api/pizzas/");

        return View(pizzas ?? new List<Pizza>());
    }
}
