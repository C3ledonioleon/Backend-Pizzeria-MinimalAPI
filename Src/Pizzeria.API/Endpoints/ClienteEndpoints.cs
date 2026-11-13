using System.Security.Claims;
using Pizzeria.API.DTOs;
using Pizzeria.API.Models;
using Pizzeria.API.Services.IServices;

namespace Pizzeria.API.Endpoints;

public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this WebApplication app)
    {
        var cliente = app.MapGroup("/api/clientes");

        cliente.WithTags("Cliente");


        cliente.MapGet("/", async (IClienteService service) =>
        {
            var clientes = await service.ObtenerTodosAsync();
            return Results.Ok(clientes);
        });


        cliente.MapGet("/{id:int}", async (
            int id,
            IClienteService service) =>
        {
            var cliente = await service.ObtenerPorIdAsync(id);

            return cliente is null
                ? Results.NotFound()
                : Results.Ok(cliente);
        });


        cliente.MapPost("/", async (CreateClienteDto dto,ClaimsPrincipal usuario,IClienteService service) =>
    {
        var idUsuarioClaim = usuario.FindFirst(ClaimTypes.NameIdentifier);

        if (idUsuarioClaim is null)
            return Results.Unauthorized();

        int idUsuario = int.Parse(idUsuarioClaim.Value);

        var clienteCreado = await service.CrearAsync(idUsuario, dto);

        return Results.Created(
            $"/api/clientes/{clienteCreado.IdCliente}",
            clienteCreado);
    })
    .RequireAuthorization();

        cliente.MapPut("/{id:int}", async (
            int id,
            UpdateClienteDto dto,
            IClienteService service) =>
        {
            var actualizado = await service.ActualizarAsync(id, dto);

            return actualizado
                ? Results.Ok()
                : Results.NotFound();
        });


        cliente.MapDelete("/{id:int}", async (
            int id,
            IClienteService service) =>
        {
            var eliminado = await service.EliminarAsync(id);

            return eliminado
                ? Results.Ok()
                : Results.NotFound();
        });
    }
}