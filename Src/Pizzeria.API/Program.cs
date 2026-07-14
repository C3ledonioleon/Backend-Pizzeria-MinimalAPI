using Pizzeria.API.Endpoints;
using Pizzeria.API.Services;
using Pizzeria.API.Sockets;
using Pizzeria.API.Data;
using Pizzeria.API.Repositories;
using Pizzeria.API.Repositories.IRepositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Registro de servicios
// =======================
// Data / repositories
builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPizzaRepository, PizzaRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

// Application services (scoped)
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<PizzaService>();
builder.Services.AddScoped<PedidoService>();

// Sockets/clients
builder.Services.AddSingleton<CocinaSocketClient>();

builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI (used by Scalar)
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =======================
// Endpoints (registrar antes de generar el JSON OpenAPI)
// =======================
app.MapClienteEndpoints();
app.MapPizzaEndpoints();
app.MapPedidoEndpoints();

// Servir solo el JSON OpenAPI (no mostrar Swagger UI)
app.UseSwagger();

// Map Scalar and point it at the Swagger JSON produced by Swashbuckle
app.MapScalarApiReference(options => options.WithOpenApiRoutePattern("/swagger/v1/swagger.json"));
app.MapGet("/", () => Results.Redirect("/scalar", false));

app.Run();