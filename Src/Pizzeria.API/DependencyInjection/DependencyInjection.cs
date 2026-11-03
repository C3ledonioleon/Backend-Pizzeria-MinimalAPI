using Pizzeria.API.Data;
using Pizzeria.API.Repositories;
using Pizzeria.API.Repositories.IRepositories;
using Pizzeria.API.Services;
using Pizzeria.API.Services.IServices;
using Pizzeria.API.Sockets;

namespace Pizzeria.API.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        
        // Data
        services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();


        // Repositories
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IPizzaRepository, PizzaRepository>();
        services.AddScoped<IDetallePedidoRepository, DetallePedidoRepository>();

        // Nuevo repositorio de Usuario
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // Services
        services.AddScoped<IPedidoService, PedidoService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IPizzaService, PizzaService>();

        // Nuevo servicio de Usuario
        services.AddScoped<IUsuarioService, UsuarioService>();

        // Socket cocina
        services.AddSingleton<CocinaSocketClient>();

        return services;
    }
}
