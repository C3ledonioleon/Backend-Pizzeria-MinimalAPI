using Pizzeria.API.DTOs;
using Pizzeria.API.Enums;
using Pizzeria.API.Models;
using Pizzeria.API.Services.IServices;
using Pizzeria.API.Sockets;
using Pizzeria.API.Repositories.IRepositories;


namespace Pizzeria.API.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IDetallePedidoRepository _detalleRepository;
    private readonly CocinaSocketClient _cocinaSocketClient;


    public Task<List<Pedido>> ObtenerTodosAsync() => _pedidoRepository.ObtenerTodosAsync();

    public Task<Pedido?> ObtenerPorIdAsync(int id) => _pedidoRepository.ObtenerPorIdAsync(id);

    public async Task<Pedido> CrearAsync(CreatePedidoDto dto)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(dto.IdCliente)
            ?? throw new ArgumentException($"No existe un cliente con id {dto.IdCliente}");

        var pedido = new Pedido(dto.IdCliente);
        decimal total = 0;

        foreach (var detalleDto in dto.Detalles)
        {
            var pizza = await _pizzaRepository.ObtenerPorIdAsync(detalleDto.IdPizza)
                ?? throw new ArgumentException($"No existe una pizza con id {detalleDto.IdPizza}");

            var detalle = new DetallePedido(
                0,
                pizza.IdPizza,
                detalleDto.Cantidad,
                pizza.Precio,
                detalleDto.Observaciones);

            pedido.Detalles.Add(detalle);
            total += pizza.Precio * detalleDto.Cantidad;
        }

        pedido.Total = total;

        var pedidoCreado = await _pedidoRepository.CrearAsync(pedido);

        try
        {
            await _cocinaSocketClient.NotificarNuevoPedidoAsync(pedidoCreado);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AVISO] No se pudo notificar a la cocina: {ex.Message}");
        }

        return pedidoCreado;
    }

    public async Task<Pedido?> CambiarEstadoAsync(int idPedido, EstadoPedido nuevoEstado)
    {
        var pedido = await _pedidoRepository.ObtenerPorIdAsync(idPedido);
        if (pedido is null) return null;

        await _pedidoRepository.ActualizarEstadoAsync(idPedido, (int)nuevoEstado);
        pedido.Estado = nuevoEstado;
        return pedido;
    }

    public Task<int> EliminarAsync(int id) => _pedidoRepository.EliminarAsync(id);

    // =======================
    // Detalles del pedido
    // =======================

    public async Task<Pedido?> AgregarDetalleAsync(int idPedido, DetallePedidoDto dto)
    {
        var pedido = await _pedidoRepository.ObtenerPorIdAsync(idPedido);
        if (pedido is null) return null;

        var pizza = await _pizzaRepository.ObtenerPorIdAsync(dto.IdPizza)
            ?? throw new ArgumentException($"No existe una pizza con id {dto.IdPizza}");

        var detalle = new DetallePedido(idPedido, dto.IdPizza, dto.Cantidad, pizza.Precio, dto.Observaciones);
        await _detalleRepository.AgregarAsync(detalle);

        await RecalcularTotalAsync(idPedido);
        return await _pedidoRepository.ObtenerPorIdAsync(idPedido);
    }

    public async Task<Pedido?> ActualizarDetalleAsync(int idPedido, int idDetalle, ActualizarDetalleDto dto)
    {
        var detalle = await _detalleRepository.ObtenerPorIdAsync(idDetalle);
        if (detalle is null || detalle.IdPedido != idPedido) return null;

        await _detalleRepository.ActualizarAsync(idDetalle, dto.Cantidad, dto.Observaciones);
        await RecalcularTotalAsync(idPedido);
        return await _pedidoRepository.ObtenerPorIdAsync(idPedido);
    }

    public async Task<Pedido?> EliminarDetalleAsync(int idPedido, int idDetalle)
    {
        var detalle = await _detalleRepository.ObtenerPorIdAsync(idDetalle);
        if (detalle is null || detalle.IdPedido != idPedido) return null;

        await _detalleRepository.EliminarAsync(idDetalle);
        await RecalcularTotalAsync(idPedido);
        return await _pedidoRepository.ObtenerPorIdAsync(idPedido);
    }

    private async Task RecalcularTotalAsync(int idPedido)
    {
        var detalles = await _detalleRepository.ObtenerPorPedidoAsync(idPedido);
        var nuevoTotal = detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

        var pedido = await _pedidoRepository.ObtenerPorIdAsync(idPedido);
        if (pedido is not null)
        {
            pedido.Total = nuevoTotal;
            await _pedidoRepository.ActualizarAsync(idPedido, pedido);
        }
    }
}