using Pizzeria.API.DTOs;
using Pizzeria.API.Enums;
using Pizzeria.API.Models;
using Pizzeria.API.Services.IServices;
using Pizzeria.API.Repositories.IRepositories;

namespace Pizzeria.API.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IDetallePedidoRepository _detalleRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IClienteRepository clienteRepository,
        IPizzaRepository pizzaRepository,
        IDetallePedidoRepository detalleRepository,
        IUsuarioRepository usuarioRepository)
    {
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
        _pizzaRepository = pizzaRepository;
        _detalleRepository = detalleRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<Pedido>> ObtenerTodosAsync()
    {
        var pedidos = await _pedidoRepository.ObtenerTodosAsync();

        foreach (var pedido in pedidos)
        {
            pedido.Detalles =
                await _detalleRepository.ObtenerPorPedidoAsync(pedido.IdPedido);
        }

        return pedidos;
    }

    public async Task<List<Pedido>> ObtenerPorUsuarioAsync(int idUsuario)
    {
        var pedidos = await _pedidoRepository.ObtenerPorUsuarioAsync(idUsuario);

        foreach (var pedido in pedidos)
        {
            pedido.Detalles =
                await _detalleRepository.ObtenerPorPedidoAsync(pedido.IdPedido);
        }

        return pedidos;
    }

    public async Task<Pedido?> ObtenerPorIdAsync(int id)
    {
        var pedido = await _pedidoRepository.ObtenerPorIdAsync(id);

        if (pedido is null)
            return null;

        pedido.Detalles =
            await _detalleRepository.ObtenerPorPedidoAsync(id);

        return pedido;
    }

    // Crear pedido.
    public async Task<Pedido> CrearAsync(
        int idUsuario,
        CreatePedidoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) ||
            string.IsNullOrWhiteSpace(dto.Apellido) ||
            string.IsNullOrWhiteSpace(dto.Telefono) ||
            string.IsNullOrWhiteSpace(dto.Direccion))
        {
            throw new ArgumentException(
                "Todos los datos del cliente son obligatorios.");
        }

        if (dto.Detalles == null || !dto.Detalles.Any())
        {
            throw new ArgumentException(
                "El pedido debe contener al menos una pizza.");
        }

        if (!Enum.IsDefined(dto.MedioPago))
        {
            throw new ArgumentException(
                "El medio de pago no es válido.");
        }

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(idUsuario)
            ?? throw new ArgumentException("No se encontró el usuario.");

        var cliente = await _clienteRepository.ObtenerPorUsuarioAsync(idUsuario);

        // Crear cliente solamente si todavía no existe.
        if (cliente is null)
        {
            cliente = new Cliente(
                dto.Nombre.Trim(),
                dto.Apellido.Trim(),
                usuario.Email,
                dto.Telefono.Trim(),
                dto.Direccion.Trim())
            {
                IdUsuario = idUsuario
            };

            cliente = await _clienteRepository.CrearAsync(cliente);
        }

        var pedido = new Pedido(cliente.IdCliente)
        {
            MedioPago = dto.MedioPago
        };

        decimal total = 0;

        foreach (var detalleDto in dto.Detalles)
        {
            if (detalleDto.Cantidad <= 0)
            {
                throw new ArgumentException(
                    "La cantidad debe ser mayor a cero.");
            }

            var pizza = await _pizzaRepository.ObtenerPorIdAsync(
                detalleDto.IdPizza);

            if (pizza is null)
            {
                throw new ArgumentException(
                    $"No existe una pizza con id {detalleDto.IdPizza}.");
            }

            var detalle = new DetallePedido(
                0,
                pizza.IdPizza,
                detalleDto.Cantidad,
                pizza.Precio,
                detalleDto.Observaciones ?? string.Empty);

            pedido.Detalles.Add(detalle);

            total += pizza.Precio * detalleDto.Cantidad;
        }

        pedido.Total = total;

        // Guardar pedido.
        var pedidoCreado = await _pedidoRepository.CrearAsync(pedido);

        // Guardar detalles.
        foreach (var detalle in pedido.Detalles)
        {
            detalle.IdPedido = pedidoCreado.IdPedido;

            await _detalleRepository.AgregarAsync(detalle);
        }

        // Recuperar detalles guardados.
        pedidoCreado.Detalles =
            await _detalleRepository.ObtenerPorPedidoAsync(
                pedidoCreado.IdPedido);

        return pedidoCreado;
    }

    // Cambiar estado.
    public async Task<Pedido?> CambiarEstadoAsync(
        int idPedido,
        EstadoPedido nuevoEstado)
    {
        if (!Enum.IsDefined(nuevoEstado))
            throw new ArgumentOutOfRangeException(
                nameof(nuevoEstado),
                "El estado del pedido no es válido.");

        var pedido = await _pedidoRepository.ObtenerPorIdAsync(idPedido);

        if (pedido is null)
            return null;

        await _pedidoRepository.ActualizarEstadoAsync(
            idPedido,
            (int)nuevoEstado);

        pedido.Estado = nuevoEstado;

        return pedido;
    }

    public Task<int> EliminarAsync(int id)
        => _pedidoRepository.EliminarAsync(id);

    // Agregar detalle.
    public async Task<Pedido?> AgregarDetalleAsync(
        int idPedido,
        DetallePedidoDto dto)
    {
        var pedido = await _pedidoRepository.ObtenerPorIdAsync(idPedido);

        if (pedido is null)
            return null;

        var pizza = await _pizzaRepository.ObtenerPorIdAsync(dto.IdPizza)
            ?? throw new ArgumentException("La pizza no existe.");

        if (dto.Cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        var detalle = new DetallePedido(
            idPedido,
            dto.IdPizza,
            dto.Cantidad,
            pizza.Precio,
            dto.Observaciones ?? string.Empty);

        await _detalleRepository.AgregarAsync(detalle);

        await RecalcularTotalAsync(idPedido);

        return await ObtenerPorIdAsync(idPedido);
    }

    public async Task<Pedido?> ActualizarDetalleAsync(
        int idPedido,
        int idDetalle,
        ActualizarDetalleDto dto)
    {
        var detalle = await _detalleRepository.ObtenerPorIdAsync(idDetalle);

        if (detalle is null || detalle.IdPedido != idPedido)
            return null;

        if (dto.Cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        await _detalleRepository.ActualizarAsync(
            idDetalle,
            dto.Cantidad,
            dto.Observaciones);

        await RecalcularTotalAsync(idPedido);

        return await ObtenerPorIdAsync(idPedido);
    }

    public async Task<Pedido?> EliminarDetalleAsync(
        int idPedido,
        int idDetalle)
    {
        var detalle = await _detalleRepository.ObtenerPorIdAsync(idDetalle);

        if (detalle is null || detalle.IdPedido != idPedido)
            return null;

        await _detalleRepository.EliminarAsync(idDetalle);

        await RecalcularTotalAsync(idPedido);

        return await ObtenerPorIdAsync(idPedido);
    }

    private async Task RecalcularTotalAsync(int idPedido)
    {
        var detalles =
            await _detalleRepository.ObtenerPorPedidoAsync(idPedido);

        var nuevoTotal = detalles.Sum(
            d => d.Cantidad * d.PrecioUnitario);

        var pedido = await _pedidoRepository.ObtenerPorIdAsync(idPedido);

        if (pedido is not null)
        {
            pedido.Total = nuevoTotal;

            await _pedidoRepository.ActualizarAsync(
                idPedido,
                pedido);
        }
    }
}
