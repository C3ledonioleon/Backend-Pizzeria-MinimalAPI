using Pizzeria.API.Endpoints;
using Pizzeria.API.Data;
using Pizzeria.API.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);



// Dependency Injection

builder.Services.AddDependencies();

// Endpoints 
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    // Configura los metadatos del documento (Título, Versión, Descripción)
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Pizzeria API";
        document.Info.Version = "v1";
        document.Info.Description = "API para la gestión de pedidos de una pizzería.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Expone el JSON nativo en /openapi/v1.json
    app.MapOpenApi(); 
    
    // Renderiza la interfaz de Scalar usando los datos nativos
    app.MapScalarApiReference(); 
}

// Endpoints

app.MapClienteEndpoints();
app.MapPizzaEndpoints();
app.MapPedidoEndpoints();


// Redirección de la raíz a la documentación de Scalar
app.MapGet("/", () => Results.Redirect("/scalar/v1", false));

app.Run();

