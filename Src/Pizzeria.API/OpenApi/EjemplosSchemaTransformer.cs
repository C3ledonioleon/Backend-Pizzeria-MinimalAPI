
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pizzeria.API.OpenApi;

public class EjemplosSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties is null)
            return;

        foreach (var propiedad in schema.Properties)
        {
            var nombreCampo = propiedad.Key;
            var esquemaCampo = propiedad.Value;

            if (esquemaCampo.Example is not null)
                continue;

            if (esquemaCampo.Type == "string" && esquemaCampo.Format is null)
            {
                esquemaCampo.Example = new OpenApiString($"Ejemplo {nombreCampo}");
            }
            else if (esquemaCampo.Type == "integer")
            {
                esquemaCampo.Example = new OpenApiInteger(1);
            }
            else if (esquemaCampo.Type == "number")
            {
                esquemaCampo.Example = new OpenApiDouble(100);
            }
        }
    }
}