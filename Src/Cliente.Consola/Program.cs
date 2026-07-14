using Cliente.Consola.Services;

ApiService api = new();

bool salir = false;

while (!salir)
{
    Console.Clear();

    Console.WriteLine("====== PIZZERIA ======");
    Console.WriteLine("1 - Crear Pedido");
    Console.WriteLine("2 - Ver Pedidos");
    Console.WriteLine("3 - Ver Clientes");
    Console.WriteLine("4 - Salir");

    Console.Write("\nOpcion: ");

    string? opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await api.CrearPedidoAsync();
            break;

        case "2":
            await api.VerPedidosAsync();
            break;

        case "3":
            await api.VerClientesAsync();
            break;

        case "4":
            salir = true;
            break;
    }

    Console.WriteLine("\nPresione una tecla...");
    Console.ReadKey();
}