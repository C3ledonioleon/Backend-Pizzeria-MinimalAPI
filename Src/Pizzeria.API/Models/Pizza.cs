namespace Pizzeria.API.Models;

public class Pizza
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Descripcion { get; set; }
    public List<string> Ingredientes { get; set; } = new();

    public Pizza() { }

    public Pizza(string nombre, decimal precio, string descripcion)
    {
        Nombre = nombre;
        Precio = precio;
        Descripcion = descripcion;
    }

    public override string ToString() => $"{Nombre} - ${Precio}";
}