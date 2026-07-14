using Reparto.Consola ;

Console.Title = "Reparto";

Console.WriteLine("================================");
Console.WriteLine(" Servicio Reparto iniciado");
Console.WriteLine(" Puerto: 5001");
Console.WriteLine("================================");

var servidor = new SocketServer();
await servidor.IniciarAsync();