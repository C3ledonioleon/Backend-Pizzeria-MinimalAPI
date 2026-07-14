using Pizzeria.API.DTOs;
using Pizzeria.API.Services;

namespace Pizzeria.API.Endpoints;

public static class SucursalEndpoints
{
    public static void MapSucursalEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/sucursales");

        grupo.MapGet("/", async (SucursalService service) =>
            Results.Ok(await service.ObtenerTodasAsync()));

        grupo.MapGet("/{id}", async (int id, SucursalService service) =>
        {
            var sucursal = await service.ObtenerPorIdAsync(id);
            return sucursal is not null ? Results.Ok(sucursal) : Results.NotFound();
        });

        grupo.MapPost("/", async (CreateSucursalDto dto, SucursalService service) =>
        {
            var sucursal = await service.CrearAsync(dto);
            return Results.Created($"/sucursales/{sucursal.IdSucursal}", sucursal);
        });

        grupo.MapPut("/{id}", async (int id, CreateSucursalDto dto, SucursalService service) =>
        {
            var filasAfectadas = await service.ActualizarAsync(id, dto);
            return filasAfectadas > 0 ? Results.NoContent() : Results.NotFound();
        });

        grupo.MapDelete("/{id}", async (int id, SucursalService service) =>
        {
            var filasAfectadas = await service.EliminarAsync(id);
            return filasAfectadas > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}