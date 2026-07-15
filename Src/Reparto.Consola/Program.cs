using Reparto.Consola;

Console.Title = "Reparto";

Console.WriteLine("================================");
Console.WriteLine(" Servicio Reparto iniciado");
Console.WriteLine(" Puerto: 6001");
Console.WriteLine("================================");

var servidor = new SocketServer();
await servidor.IniciarAsync();