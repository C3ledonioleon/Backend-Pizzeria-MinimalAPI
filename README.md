
# Integrantes: 
 * Celedonio, Leon Flores
 * Eric, Aguirre

# Pizzeria API - Sistema Distribuido

Sistema de gestión de pedidos para pizzería desarrollado como proyecto integrador de Software (ET12 DE1 - "Lib. Gral. José de San Martín"). Implementa una arquitectura distribuida con API REST y comunicación por sockets TCP entre servicios independientes.

## Descripción

El sistema simula el flujo completo de una pizzería digital: un cliente realiza un pedido a través de una API REST, el backend lo registra en base de datos y notifica automáticamente al servicio de cocina mediante sockets. Una vez preparado el pedido, cocina notifica al servicio de reparto para la entrega, todo de forma asincrónica y con manejo de errores ante fallos de red.

## Arquitectura

El proyecto está compuesto por 4 aplicaciones independientes que se comunican entre sí:

| Proyecto | Tipo | Función | Puerto |
|---|---|---|---|
| `Pizzeria.API` | Web API (Minimal API) | Backend central: gestiona clientes, pizzas, pedidos y persistencia | 5260 |
| `Cliente.Consola` | Consola | Simula al cliente final que realiza pedidos vía HTTP | - |
| `Cocina.Consola` | Consola (Socket Server) | Recibe pedidos por socket, simula preparación | 6000 |
| `Reparto.Consola` | Consola (Socket Server) | Recibe aviso de pedido listo, simula entrega | 6001 |

## Tecnologías

- **.NET 8** - Framework principal
- **C# Minimal API** - Backend REST
- **MySQL** - Base de datos relacional
- **Dapper** - Micro ORM para acceso a datos
- **MySqlConnector** - Driver de conexión a MySQL
- **Scalar** - Documentación interactiva de la API
- **Sockets TCP** - Comunicación entre servicios internos
- **Async/Await** - Programación asincrónica en toda la solución

## Estructura del proyecto

```
Backend-Pizzeria-MinimalAPI/
│
├── Scripts/
│   ├── Script.sql              # Script de creación de tablas
│   └── INSERT.sql              # Datos iniciales de prueba
│
├── Src/
│   │
│   ├── Cliente.Consola/        # Aplicación de consola para clientes
│   │
│   ├── Cocina.Consola/         # Aplicación de consola para cocina
│   │                              (Servidor Socket)
│   │
│   ├── Reparto.Consola/        # Aplicación de consola para reparto
│   │                              (Servidor Socket)
│   │
│   └── Pizzeria.API/           # Backend principal - Minimal API
│       │
│       ├── Enums/              # Enumeraciones del sistema
│       │                         (EstadoPedido, RolEmpleado)
│       │
│       ├── Models/             # Entidades del dominio
│       │
│       ├── DTOs/               # Objetos de transferencia de datos
│       │
│       ├── Validators/         # Validaciones de datos de entrada
│       │
│       ├── Services/           # Lógica de negocio de la aplicación
│       │
│       ├── Repositories/       # Acceso a datos mediante Dapper
│       │
│       ├── Sockets/            # Comunicación mediante sockets
│       │
│       ├── Endpoints/          # Definición de endpoints de la API
│       │
│       ├── Data/               # Configuración y conexión a la base de datos
│       │
│       └── Program.cs          # Configuración principal de la API
│
└── MinimalAPI.sln              # Solución principal del proyecto
```

## Modelo de datos

**Entidades principales:**
- `Cliente` - Datos de contacto del comprador (sin autenticación)
- `Pizza` - Productos del menú
- `Pedido` - Cabecera del pedido (fecha, estado, cliente, total)
- `DetallePedido` - Ítems del pedido (pizza, cantidad, precio congelado)

**Estados del pedido:**

## Cómo ejecutar el proyecto

### Requisitos previos
- .NET 8 SDK
- MySQL Server corriendo localmente

### 1. Base de datos

Ejecutar en MySQL, en este orden:
```bash
mysql -u root -p < Scripts/Script.sql
mysql -u root -p < Scripts/INSERT.sql
```

### 2. Configurar la cadena de conexión

En `Src/Pizzeria.API/appsettings.json`, ajustar usuario y contraseña reales:
```json
{
  "ConnectionStrings": {
    "PizzeriaDB": "Server=localhost;Port=3306;Database=PizzeriaDB;User=root;Password=;"
  }
}
```

### 3. Levantar los servicios (en este orden, cada uno en su propia terminal)

```bash
# Terminal 1
cd Src/Cocina.Consola
dotnet run

# Terminal 2
cd Src/Reparto.Consola
dotnet run

# Terminal 3
cd Src/Pizzeria.API
dotnet run

# Terminal 4
cd Src/Cliente.Consola
dotnet run
```

### 4. Probar la API directamente

Con `Pizzeria.API` corriendo, abrir en el navegador:

## 🔌 Endpoints principales

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/clientes` | Lista todos los clientes |
| `POST` | `/clientes` | Crea un cliente nuevo |
| `PUT` | `/clientes/{id}` | Actualiza un cliente |
| `DELETE` | `/clientes/{id}` | Elimina un cliente |
| `GET` | `/pizzas` | Lista el menú de pizzas |
| `POST` | `/pizzas` | Crea una pizza nueva |
| `GET` | `/pedidos` | Lista todos los pedidos |
| `POST` | `/pedidos` | Crea un pedido con sus detalles |
| `PUT` | `/pedidos/{id}/estado` | Cambia el estado de un pedido |
| `POST` | `/pedidos/{idPedido}/detalles` | Agrega un ítem a un pedido existente |
| `PUT` | `/pedidos/{idPedido}/detalles/{idDetalle}` | Modifica un ítem del pedido |
| `DELETE` | `/pedidos/{idPedido}/detalles/{idDetalle}` | Elimina un ítem del pedido |

## Comunicación por sockets

Cuando se crea un pedido (`POST /pedidos`), `Pizzeria.API` actúa como **cliente de socket** y notifica a `Cocina.Consola` (que actúa como **servidor**, escuchando en el puerto 6000). Una vez que cocina simula la preparación, notifica a su vez a `Reparto.Consola` (servidor en el puerto 6001) para simular la entrega.

Si algún servicio de socket no está disponible, el sistema captura la excepción, informa por consola, y el pedido queda igualmente registrado en la base de datos (no se pierde información ante una caída de un servicio interno).

## Manejo de errores

- Validación de datos de entrada en cada endpoint antes de procesar la solicitud
- Timeout de conexión (3 segundos) al notificar a servicios por socket
- Captura de excepciones de red sin interrumpir el flujo principal del pedido
- Respuestas HTTP semánticas (`400`, `404`, `201`) según el resultado de cada operación

## 👤 Autor

Proyecto desarrollado para la materia Programacion Sobre Redes - 6to año - ET12 DE1