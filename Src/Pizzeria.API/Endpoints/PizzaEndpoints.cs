using Pizzeria.API.DTOs;
using Pizzeria.API.Services.IServices;

namespace Pizzeria.API.Endpoints;
public static class PizzaEndpoints
{
    public static void MapPizzaEndpoints(this WebApplication app)
    {
        var pizza = app.MapGroup("/api/pizzas");

        pizza.WithTags("Pizza");

        pizza.MapGet("/", async (IPizzaService service) =>
        {
            var pizzas = await service.ObtenerTodasAsync();

            return Results.Ok(pizzas);
        });

        pizza.MapGet("/{id:int}", async (int id, IPizzaService service) =>
        {
            var pizza = await service.ObtenerPorIdAsync(id);

            return pizza is null
                ? Results.NotFound()
                : Results.Ok(pizza);
        });

        pizza.MapPost("/", async ( CreatePizzaDto dto, IPizzaService service) =>
        {
            var pizza = await service.CrearAsync(dto);

            return Results.Created(
                $"/api/pizzas/{pizza.IdPizza}",
                pizza);
        });



        pizza.MapPut("/{id:int}", async ( int id, CreatePizzaDto dto, IPizzaService service) =>
        {
            var filas = await service.ActualizarAsync(id, dto);

            return filas > 0
                ? Results.Ok()
                : Results.NotFound();
        });

        pizza.MapDelete("/{id:int}", async (int id, IPizzaService service) =>
        {
            var filas = await service.EliminarAsync(id);

            return filas > 0
                ? Results.Ok()
                : Results.NotFound();
        });
    }
}