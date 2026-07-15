using System.Net.Http.Json;
using System.Text.Json;

var http = new HttpClient
{
    BaseAddress = new Uri("http://localhost:5260")
};

Console.WriteLine("=== Pizzeria ===");

int? idClienteActual = null;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. Crear cliente");
    Console.WriteLine("2. Ver pizzas");
    Console.WriteLine("3. Nuevo pedido");
    Console.WriteLine("4. Ver pedidos");
    Console.WriteLine("5. Salir");
    Console.Write("Opcion: ");

    var opcion = Console.ReadLine();

    try
    {
        switch (opcion)
        {
            case "1":
                Console.Write("Nombre: ");
                var nombre = Console.ReadLine();

                Console.Write("Apellido: ");
                var apellido = Console.ReadLine();

                Console.Write("Email: ");
                var email = Console.ReadLine();

                Console.Write("Telefono: ");
                var telefono = Console.ReadLine();

                Console.Write("Direccion: ");
                var direccion = Console.ReadLine();

                var clienteBody = new{ nombre,apellido,email,telefono,direccion };

                var respuestaCliente =
                    await http.PostAsJsonAsync("/api/clientes", clienteBody);

                var contenidoCliente =
                    await respuestaCliente.Content.ReadAsStringAsync();

                if (!respuestaCliente.IsSuccessStatusCode)
                {
                    Console.WriteLine(contenidoCliente);
                    break;
                }

                var cliente =
                    JsonSerializer.Deserialize<JsonElement>(contenidoCliente);

                idClienteActual =
                    cliente.GetProperty("idCliente").GetInt32();

                Console.WriteLine( $"Cliente creado con id {idClienteActual}");
                
                break;

            case "2":

                var pizzas =
                    await http.GetFromJsonAsync<List<JsonElement>>( "/api/pizzas");

                foreach (var pizza in pizzas!)
                {
                    Console.WriteLine(
                        $"[{pizza.GetProperty("idPizza")}] " +
                        $"{pizza.GetProperty("nombre")} - " +
                        $"${pizza.GetProperty("precio")}");
                }

                break;

            case "3":

                if (idClienteActual is null)
                {
                    Console.Write("Id Cliente: ");
                    idClienteActual =
                        int.Parse(Console.ReadLine()!);
                }

                var detalles = new List<object>();

                while (true)
                {
                    var listaPizzas =
                        await http.GetFromJsonAsync<List<JsonElement>>(
                            "/api/pizzas");

                    Console.WriteLine();

                    foreach (var pizza in listaPizzas!)
                    {
                        Console.WriteLine(
                            $"[{pizza.GetProperty("idPizza")}] " +
                            $"{pizza.GetProperty("nombre")} - " +
                            $"${pizza.GetProperty("precio")}");
                    }

                    Console.Write(
                        "Id Pizza (0 terminar): ");

                    var idPizza =
                        int.Parse(Console.ReadLine()!);

                    if (idPizza == 0)
                        break;

                    Console.Write("Cantidad: ");

                    var cantidad =
                        int.Parse(Console.ReadLine()!);

                    Console.Write("Observaciones: ");

                    var observaciones =
                        Console.ReadLine();

                    detalles.Add(new
                    {
                        idPizza,
                        cantidad,
                        observaciones
                    });
                }

                if (detalles.Count == 0)
                {
                    Console.WriteLine(
                        "Pedido cancelado");
                    break;
                }

                var pedidoBody = new {idCliente = idClienteActual,detalles};

                var respuestaPedido = await http.PostAsJsonAsync("/api/pedidos",pedidoBody);

                var contenidoPedido =
                    await respuestaPedido.Content.ReadAsStringAsync();

                if (!respuestaPedido.IsSuccessStatusCode)
                {
                    Console.WriteLine(
                        contenidoPedido);
                    break;
                }
                var pedido = JsonSerializer.Deserialize<JsonElement>(contenidoPedido);

                Console.WriteLine($"Pedido #{pedido.GetProperty("idPedido")} creado");
                
                Console.WriteLine($"Total: ${pedido.GetProperty("total")}");

                break;
            case "4":

                var pedidos =
                    await http.GetFromJsonAsync<List<JsonElement>>(
                        "/api/pedidos");

                foreach (var p in pedidos!)
                {
                    Console.WriteLine(
                        $"Pedido #{p.GetProperty("idPedido")} - " +
                        $"Cliente {p.GetProperty("idCliente")} - " +
                        $"Estado {p.GetProperty("estado")} - " +
                        $"Total ${p.GetProperty("total")}");
                }

                break;

            case "5":
                return;

            default:
                Console.WriteLine("Opcion invalida");
                break;
        }
    }
    catch(Exception ex)
    {
        Console.WriteLine(
            $"Error: {ex.Message}");
    }
}