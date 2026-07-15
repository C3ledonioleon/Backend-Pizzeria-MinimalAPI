using System.Net.Http.Json;
using System.Text.Json;

var http = new HttpClient { BaseAddress = new Uri("http://localhost:5260") };

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
        if (opcion == "1")
        {
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

            var body = new { nombre, apellido, email, telefono, direccion };
            var respuesta = await http.PostAsJsonAsync("/clientes", body);
            var cliente = await respuesta.Content.ReadFromJsonAsync<JsonElement>();

            idClienteActual = cliente.GetProperty("idCliente").GetInt32();
            Console.WriteLine($"Cliente creado con id {idClienteActual}.");
        }
        else if (opcion == "2")
        {
            var pizzas = await http.GetFromJsonAsync<List<JsonElement>>("/pizzas");
            Console.WriteLine();
            foreach (var pizza in pizzas!)
            {
                Console.WriteLine($"[{pizza.GetProperty("idPizza")}] {pizza.GetProperty("nombre")} - ${pizza.GetProperty("precio")}");
            }
        }
        else if (opcion == "3")
        {
            if (idClienteActual is null)
            {
                Console.Write("Ingresa tu IdCliente: ");
                idClienteActual = int.Parse(Console.ReadLine()!);
            }

            var detalles = new List<object>();

            while (true)
            {
                var pizzas = await http.GetFromJsonAsync<List<JsonElement>>("/pizzas");
                Console.WriteLine();
                foreach (var pizza in pizzas!)
                {
                    Console.WriteLine($"[{pizza.GetProperty("idPizza")}] {pizza.GetProperty("nombre")} - ${pizza.GetProperty("precio")}");
                }

                Console.Write("Id de la pizza (0 para terminar): ");
                var idPizza = int.Parse(Console.ReadLine()!);
                if (idPizza == 0) break;

                Console.Write("Cantidad: ");
                var cantidad = int.Parse(Console.ReadLine()!);

                Console.Write("Observaciones (opcional): ");
                var observaciones = Console.ReadLine();

                detalles.Add(new { idPizza, cantidad, observaciones });
            }

            if (detalles.Count == 0)
            {
                Console.WriteLine("Pedido cancelado, no tenia items.");
                continue;
            }

            var pedidoBody = new { idCliente = idClienteActual, detalles };
            var respuesta = await http.PostAsJsonAsync("/pedidos", pedidoBody);

            if (!respuesta.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {await respuesta.Content.ReadAsStringAsync()}");
                continue;
            }

            var pedido = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
            Console.WriteLine($"Pedido #{pedido.GetProperty("idPedido")} creado. Total: ${pedido.GetProperty("total")}");
        }
        else if (opcion == "4")
        {
            var pedidos = await http.GetFromJsonAsync<List<JsonElement>>("/pedidos");
            Console.WriteLine();
            foreach (var pedido in pedidos!)
            {
                Console.WriteLine($"Pedido #{pedido.GetProperty("idPedido")} - Cliente {pedido.GetProperty("idCliente")} - Estado: {pedido.GetProperty("estado")} - Total: ${pedido.GetProperty("total")}");
            }
        }
        else if (opcion == "5")
        {
            break;
        }
        else
        {
            Console.WriteLine("Opcion invalida.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}