namespace Pizzeria.API.DTOs;

public class CreatePizzaDto
{
    public string Nombre { get; set; } 
    public decimal Precio { get; set; }
    public string Descripcion { get; set; }
    public List<string> Ingredientes { get; set; } = new();
}