public class ErrorResponseDto
{
    public string Mensaje { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public int Codigo { get; set; }
}