using Pizzeria.API.DTOs;
using Pizzeria.API.Services;
using Pizzeria.API.Validators;

namespace Pizzeria.API.Endpoints;

public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/clientes");
        grupo.WithTags("Clientes");

        grupo.MapGet("/", (ClienteService service) =>
            Results.Ok(service.ObtenerTodos()));

        grupo.MapGet("/{id}", (int id, ClienteService service) =>
        {
            var cliente = service.ObtenerPorId(id);
            return cliente is not null ? Results.Ok(cliente) : Results.NotFound();
        });

        grupo.MapPost("/", (CreateClienteDto dto, ClienteService service) =>
        {
            var errores = ClienteValidator.Validar(dto);
            if (errores.Count > 0)
                return Results.BadRequest(errores);

            var cliente = service.Crear(dto);
            return Results.Created($"/clientes/{cliente.IdCliente}", cliente);
        });
    }
}

