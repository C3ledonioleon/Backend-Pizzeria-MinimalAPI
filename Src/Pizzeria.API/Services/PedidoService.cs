using Pizzeria.API.DTOs;
using Pizzeria.API.Enums;
using Pizzeria.API.Models;
using Pizzeria.API.Repositories.IRepositories;
using Pizzeria.API.Sockets;


namespace Pizzeria.API.Services;

public class PedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IPizzaRepository _pizzaRepository;
    private readonly CocinaSocketClient _cocinaSocketClient;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IClienteRepository clienteRepository,
        IPizzaRepository pizzaRepository,
        CocinaSocketClient cocinaSocketClient)
    {
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
        _pizzaRepository = pizzaRepository;
        _cocinaSocketClient = cocinaSocketClient;
    }

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
}