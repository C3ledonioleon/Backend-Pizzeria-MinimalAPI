namespace MVC.Pizzeria.Models;

public class Pizza
{
    public int IdPizza { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public List<string> Ingredientes { get; set; } = new();
}
