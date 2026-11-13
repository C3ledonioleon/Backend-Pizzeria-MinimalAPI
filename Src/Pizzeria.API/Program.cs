using Pizzeria.API.Endpoints;
using Pizzeria.API.Data;
using Pizzeria.API.DependencyInjection;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Dependency Injection

builder.Services.AddDependencies();

// AUTENTICACIÓN JWT

var jwtKey = builder.Configuration["Jwt:Key"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

// Endpoints

app.MapClienteEndpoints();
app.MapPizzaEndpoints();
app.MapPedidoEndpoints();
app.MapUsuarioEndpoints();

// Redirección de la raíz a la documentación de Scalar
app.MapGet("/", () => Results.Redirect("/scalar/v1", false));

app.Run();
