using Pizzeria.API.DTOs;
using Pizzeria.API.Services;
using Pizzeria.API.Validators;

namespace Pizzeria.API.Endpoints;

public static class PizzaEndpoints
{
    public static void MapPizzaEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/pizzas");

        grupo.MapGet("/", async (PizzaService service) =>
            Results.Ok(await service.ObtenerTodasAsync()));

        grupo.MapGet("/{id}", async (int id, PizzaService service) =>
        {
            var pizza = await service.ObtenerPorIdAsync(id);
            return pizza is not null ? Results.Ok(pizza) : Results.NotFound();
        });

        grupo.MapPost("/", async (CreatePizzaDto dto, PizzaService service) =>
        {
            var errores = PizzaValidator.Validar(dto);
            if (errores.Count > 0)
                return Results.BadRequest(errores);

            var pizza = await service.CrearAsync(dto);
            return Results.Created($"/pizzas/{pizza.IdPizza}", pizza);
        });

        grupo.MapPut("/{id}", async (int id, CreatePizzaDto dto, PizzaService service) =>
        {
            var errores = PizzaValidator.Validar(dto);
            if (errores.Count > 0)
                return Results.BadRequest(errores);

            var filasAfectadas = await service.ActualizarAsync(id, dto);
            return filasAfectadas > 0 ? Results.NoContent() : Results.NotFound();
        });

        grupo.MapDelete("/{id}", async (int id, PizzaService service) =>
        {
            var filasAfectadas = await service.EliminarAsync(id);
            return filasAfectadas > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}