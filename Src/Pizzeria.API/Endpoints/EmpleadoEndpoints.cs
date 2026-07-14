using Pizzeria.API.DTOs;
using Pizzeria.API.Services;
using Pizzeria.API.Validators;



namespace Pizzeria.API.Endpoints;

public static class EmpleadoEndpoints
{
    public static void MapEmpleadoEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/empleados");

        grupo.MapGet("/", async (EmpleadoService service) =>
            Results.Ok(await service.ObtenerTodosAsync()));

        grupo.MapGet("/{id}", async (int id, EmpleadoService service) =>
        {
            var empleado = await service.ObtenerPorIdAsync(id);
            return empleado is not null ? Results.Ok(empleado) : Results.NotFound();
        });

        grupo.MapPost("/", async (CreateEmpleadoDto dto, EmpleadoService service) =>
        {
            var errores = EmpleadoValidator.Validar(dto);
            if (errores.Count > 0)
                return Results.BadRequest(errores);

            var empleado = await service.CrearAsync(dto);
            return Results.Created($"/empleados/{empleado.IdEmpleado}", empleado);
        });

        grupo.MapPut("/{id}", async (int id, CreateEmpleadoDto dto, EmpleadoService service) =>
        {
            var errores = EmpleadoValidator.Validar(dto);
            if (errores.Count > 0)
                return Results.BadRequest(errores);

            var filasAfectadas = await service.ActualizarAsync(id, dto);
            return filasAfectadas > 0 ? Results.NoContent() : Results.NotFound();
        });

        grupo.MapDelete("/{id}", async (int id, EmpleadoService service) =>
        {
            var filasAfectadas = await service.EliminarAsync(id);
            return filasAfectadas > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}