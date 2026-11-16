# Pizzería Arte & Sabor

Sistema de gestión de menú y pedidos desarrollado como proyecto integrador de Software (ET12 DE1 - “Lib. Gral. José de San Martín”). La solución web está compuesta por una API REST y una aplicación MVC que consume esa API.

## Arquitectura

La solución documentada aquí se centra en dos proyectos: la API implementa los endpoints y las reglas de acceso; la aplicación MVC presenta la interfaz para clientes y administradores y consume la API mediante `HttpClient`.

| Proyecto | Tecnología | Función |
|---|---|---|
| `Pizzeria.API` | ASP.NET Core Minimal API, .NET 10 | API REST, autenticación, autorización, lógica de aplicación y acceso a MySQL |
| `MVC.Pizzeria` | ASP.NET Core MVC, .NET 10 | Sitio web, autenticación por cookies, menú, carrito e interfaces según el rol |

### Estructura principal

```text
MinimalAPI.sln
Scripts/
  Script.sql
  INSERT.sql
  MigracionImagenPizza.sql
Src/
  MVC.Pizzeria/
    Controllers/
    Models/
    Services/                 # Cliente HTTP de la API
    Views/
    wwwroot/                  # Archivos estáticos e imágenes
  Pizzeria.API/
    Data/                     # Conexión a MySQL
    DTOs/
    Endpoints/
    Enums/
    Models/
    Repositories/              # Persistencia con Dapper
    Services/                  # Lógica de aplicación
    Validators/
    Program.cs
```

## Tecnologías y dependencias

- **C# y .NET 10** para la API y la aplicación MVC.
- **ASP.NET Core Minimal API** para la API y **ASP.NET Core MVC** para el sitio web.
- **MySQL** para persistir usuarios, perfiles de cliente, pizzas, pedidos y detalles.
- **Dapper** y **MySqlConnector** para el acceso a la base de datos.
- **JWT Bearer** para autenticar solicitudes a la API.
- **BCrypt.Net-Next** para almacenar y verificar contraseñas.
- **OpenAPI** y **Scalar.AspNetCore** para la documentación interactiva de la API.

## Funcionalidades

### Clientes

- Registrarse e iniciar sesión con usuario o correo y contraseña.
- Consultar el menú de pizzas.
- Agregar pizzas al carrito, modificar cantidades y confirmar un pedido.
- Consultar su historial con fecha y hora, productos, cantidades, total y estado.
- El historial se filtra en la API usando la identidad del usuario autenticado; un cliente no puede consultar los pedidos de otra cuenta.

### Administradores

- Crear, editar y eliminar pizzas.
- Agregar una imagen al crear o editar una pizza: subir un archivo JPG, PNG o WebP de hasta 5 MB, o indicar una URL HTTP/HTTPS.
- Consultar los pedidos de todos los clientes y cambiar el estado de cualquier pedido.
- Consultar y administrar clientes desde los endpoints habilitados para el rol.

Los controles de la interfaz MVC se muestran según el rol, pero los permisos también se hacen cumplir en la API. Ocultar una opción en la interfaz no sustituye la autorización del endpoint.

### Estados de pedidos

| Valor | Estado |
|---:|---|
| 1 | `EsperaConfirmacion` |
| 2 | `EnPreparacion` |
| 3 | `EnViaje` |
| 4 | `Entregado` |
| 5 | `Cancelado` |

## Autenticación y autorización

- Los roles almacenados en la base de datos son `Administrador = 1` y `Cliente = 2`. En los tokens se representan con los nombres `Administrador` y `Cliente`.
- La API emite JWT al iniciar sesión y los valida en las rutas protegidas.
- MVC inicia una sesión de autenticación mediante una cookie persistente y guarda el JWT en ella. Su cliente HTTP envía ese token a la API como `Bearer`.
- El carrito se mantiene mediante la sesión de ASP.NET Core MVC; es independiente de la cookie de autenticación.
- El registro y el inicio de sesión están disponibles sin autenticación. La aplicación almacena contraseñas nuevas con BCrypt. También puede verificar credenciales heredadas en texto plano o SHA-256 y migrarlas a BCrypt luego de un inicio de sesión válido.

## Endpoints principales de la API

Todas las rutas de la tabla tienen el prefijo `/api`. Las rutas protegidas requieren un JWT Bearer y el rol indicado.

| Método | Ruta | Acceso | Descripción |
|---|---|---|---|
| `POST` | `/usuarios/registro` | Público | Registrar una cuenta de cliente |
| `POST` | `/usuarios/login` | Público | Iniciar sesión y obtener un JWT |
| `GET` | `/pizzas/` | Público | Consultar el menú |
| `GET` | `/pizzas/{id}` | Público | Consultar una pizza |
| `POST` | `/pizzas/` | Administrador | Crear una pizza |
| `PUT` | `/pizzas/{id}` | Administrador | Editar una pizza |
| `DELETE` | `/pizzas/{id}` | Administrador | Eliminar una pizza |
| `GET` | `/clientes/yo` | Cliente | Consultar el perfil asociado a la cuenta autenticada |
| `POST` | `/clientes/` | Cliente | Crear el perfil de la cuenta autenticada |
| `GET` | `/clientes/` | Administrador | Consultar todos los clientes |
| `GET` | `/clientes/{id}` | Administrador | Consultar un cliente |
| `PUT` | `/clientes/{id}` | Administrador | Actualizar un cliente |
| `DELETE` | `/clientes/{id}` | Administrador | Eliminar un cliente |
| `POST` | `/pedidos/` | Cliente | Crear un pedido para la cuenta autenticada |
| `GET` | `/pedidos/mis-pedidos` | Cliente | Consultar únicamente el historial propio |
| `GET` | `/pedidos/` | Administrador | Consultar todos los pedidos |
| `GET` | `/pedidos/{id}` | Administrador | Consultar un pedido |
| `PUT` | `/pedidos/{id}/estado?estado={valor}` | Administrador | Cambiar el estado del pedido |
| `DELETE` | `/pedidos/{id}` | Administrador | Eliminar un pedido |
| `POST` | `/pedidos/{idPedido}/detalles` | Administrador | Agregar un detalle al pedido |
| `PUT` | `/pedidos/{idPedido}/detalles/{idDetalle}` | Administrador | Actualizar un detalle |
| `DELETE` | `/pedidos/{idPedido}/detalles/{idDetalle}` | Administrador | Eliminar un detalle |

La aplicación de desarrollo expone OpenAPI en `/openapi/v1.json` y la interfaz de Scalar en `/scalar/v1`.

## Base de datos e imágenes

La base de datos se llama `6to_Pizzeria`. La cadena de conexión se configura en `Src/Pizzeria.API/appsettings.json`, bajo `ConnectionStrings:6to_Pizzeria`. La API también requiere configurar `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` y los valores de expiración. Usá valores propios para cada entorno y no publiques credenciales ni claves reales.

`Scripts/Script.sql` crea la base, las tablas y datos de ejemplo, incluida una cuenta de administrador. **El script elimina y vuelve a crear `6to_Pizzeria`; no lo ejecutes sobre una base que quieras conservar.** `Scripts/INSERT.sql` contiene datos de ejemplo separados que también están incluidos en el script de inicialización; no lo ejecutes después de `Script.sql` en la misma base, porque intentaría insertar los mismos registros.

Para una base ya existente:

- Ejecutá `Scripts/MigracionImagenPizza.sql` una sola vez si todavía no tiene la columna `Pizza.ImagenUrl`.

El script de inicialización crea la cuenta de prueba `admin@gmail.com` con contraseña `Admin123!`. Es una credencial de desarrollo; cambiala antes de usar la aplicación con datos reales.

Las imágenes subidas desde MVC se guardan bajo `Src/MVC.Pizzeria/wwwroot/Images/pizzas`; las URL ingresadas se almacenan directamente. La ruta o URL queda registrada en `Pizza.ImagenUrl`.

## Requisitos y ejecución

1. Instalá el SDK de .NET 10 y MySQL Server.
2. Ejecutá `Scripts/Script.sql` en MySQL Workbench o en otra herramienta MySQL para crear una base de datos nueva. Recordá que el script elimina una base existente con el mismo nombre.
3. Configurá `ConnectionStrings:6to_Pizzeria` y los valores JWT en `Src/Pizzeria.API/appsettings.json` o mediante la configuración del entorno.
4. Iniciá la API desde la raíz de la solución:

   ```powershell
   dotnet run --project Src\Pizzeria.API\Pizzeria.API.csproj
   ```

   El perfil HTTP de desarrollo escucha en `http://localhost:5260`.

5. En otra terminal, iniciá MVC:

   ```powershell
   dotnet run --project Src\MVC.Pizzeria\MVC.Pizzeria.csproj
   ```

   El perfil HTTP de desarrollo escucha en `http://localhost:5127`. MVC está configurado para consumir la API en `http://localhost:5260`; si cambiás ese puerto, actualizá también la dirección base en `Src/MVC.Pizzeria/Program.cs`.

## Datos de referencia

El historial y la relación de pedidos se basan en las tablas `Usuario`, `Cliente`, `Pizza`, `Pedido` y `DetallePedido`. Cada perfil de cliente está asociado a una cuenta; los pedidos guardan su fecha y hora, estado, total y detalles con cantidad y precio unitario.

## Créditos

Proyecto desarrollado para la materia Programación sobre Redes - 6.º año - ET12 DE1.

Integrantes:

- Celedonio Leon Flores
- Eric Aguirre
