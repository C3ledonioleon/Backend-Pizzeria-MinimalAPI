namespace Pizzeria.API.DTOs;

public class CreateClienteDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; }  = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; }  = string.Empty;
}

public class UpdateClienteDto
{ 
    public string Email { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;
}