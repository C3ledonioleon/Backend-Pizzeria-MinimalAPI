using Pizzeria.API.Endpoints;
using Pizzeria.API.Data;
using Pizzeria.API.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);



// Data
builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();



// Dependency Injection

builder.Services.AddDependencies();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();

    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Pizzeria API",
        Version = "v1",
        Description = "API para la gestión de pedidos de una pizzería.",
    });
});

var app = builder.Build();

// Endpoints

app.MapClienteEndpoints();
app.MapPizzaEndpoints();
app.MapPedidoEndpoints();


app.UseSwagger();


app.MapScalarApiReference(options =>
    options.WithOpenApiRoutePattern("/swagger/v1/swagger.json"));


app.MapGet("/", () => Results.Redirect("/scalar", false));


app.Run();