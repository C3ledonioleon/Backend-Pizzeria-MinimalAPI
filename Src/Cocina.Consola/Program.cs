using Cocina.Consola;

Console.Title = "Cocina";

Console.WriteLine("================================");
Console.WriteLine(" Servicio Cocina iniciado");
Console.WriteLine(" Puerto: 6000");
Console.WriteLine("================================");

var servidor = new SocketServer();
await servidor.IniciarAsync();