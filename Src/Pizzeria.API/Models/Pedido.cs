using System;
using System.Collections.Generic;
using System.Linq;

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public List<Pizza> Pizzas { get; set; } = new();
    public decimal Total => CalcularTotal();
    public EstadoPedido Estado { get; set; } = EstadoPedido.EsperaConfirmacion;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaActualizacion { get; set; }

    public Pedido() { }

    public Pedido(int clienteId, List<Pizza> pizzas)
    {
        ClienteId = clienteId;
        Pizzas = pizzas ?? new();
    }

    private decimal CalcularTotal()
    {
        return Pizzas.Sum(p => p.Precio);
    }

    public void ActualizarEstado(EstadoPedido nuevoEstado)
    {
        Estado = nuevoEstado;
        FechaActualizacion = DateTime.Now;
    }
}