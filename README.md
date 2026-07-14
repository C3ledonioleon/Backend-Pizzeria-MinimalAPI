<h1 align="center">E.T. Nº12 D.E. 1º "Libertador Gral. José de San Martín"</h1>
<p align="center">
  <img src="https://et12.edu.ar/imgs/computacion/vamoaprogramabanner.png">
</p>

# Pizzería API - Minimal API

Proyecto desarrollado para la materia Programación Sobre Redes de la Escuela Técnica N.º 12 D.E. 1.

## Descripción

Este proyecto implementa una **API REST** utilizando **ASP.NET Core Minimal API** para simular el funcionamiento de una pizzería digital.

La aplicación permite que un cliente realice pedidos de pizza, los cuales son procesados por un backend encargado de administrar la lógica de negocio y comunicarse con servicios internos mediante sockets.

Además, se aplican conceptos de:

- APIs REST
- Programación distribuida
- Comunicación mediante sockets
- Programación asincrónica
- Manejo de excepciones
- Arquitectura Cliente-Servidor

---

# Objetivos

- Desarrollar una API REST utilizando Minimal API.
- Aplicar una arquitectura distribuida.
- Implementar comunicación entre módulos mediante sockets.
- Gestionar pedidos de pizzas.
- Aplicar programación asincrónica.
- Implementar manejo de errores en escenarios distribuidos.

---

# Tecnologías utilizadas

- C#
- .NET 9
- ASP.NET Core Minimal API
- Swagger / OpenAPI
- Sockets TCP
- Visual Studio 2022
- Git
- GitHub

---

# Estructura del proyecto

```
Pizzeria.API
│
├── Endpoints/
├── Models/
├── Services/
├── Sockets/
├── DTOs/
├── Program.cs
└── appsettings.json
```

---

# Estados del pedido

Los pedidos pueden encontrarse en alguno de los siguientes estados:

- Espera de confirmación
- En preparación
- En viaje
- Entregado
- Cancelado

---

# Endpoints

## Obtener todas las pizzas

```
GET /pizzas
```

---

## Obtener una pizza

```
GET /pizzas/{id}
```

---

## Crear un pedido

```
POST /pedidos
```

---

## Obtener pedidos

```
GET /pedidos
```

---

## Actualizar estado del pedido

```
PUT /pedidos/{id}/estado
```

---

# Arquitectura

```
Cliente
   │
   ▼
Minimal API
   │
   ├────────────► Servicio Cocina (Sockets)
   │
   └────────────► Servicio Reparto (Sockets)
```

---

# Flujo del sistema

1. El cliente realiza un pedido.
2. La API valida la información.
3. Se registra el pedido.
4. Se envía la solicitud al servicio de cocina.
5. La cocina confirma la preparación.
6. El pedido pasa al servicio de reparto.
7. Finalmente se entrega al cliente.

---

# Manejo de errores

La aplicación contempla situaciones como:

- Pedido inexistente.
- Pizza inexistente.
- Error de comunicación con la cocina.
- Error de conexión por sockets.
- Excepciones en procesos asincrónicos.
- Validaciones de datos.

---

# Pruebas

La API puede probarse utilizando:

- Swagger o Scalar
- Postman

---

# Cómo ejecutar

## Clonar el repositorio

```bash
git clone https://github.com/usuario/Pizzeria.API.git
```

## Ingresar al proyecto

```bash
cd Pizzeria.API
```

## Restaurar paquetes

```bash
dotnet restore
```

## Ejecutar

```bash
dotnet run
```

Luego abrir:

```
https://localhost:5001/swagger o https://localhost:5001/scalar  
```

---

# Conceptos aplicados

- Arquitectura Cliente-Servidor
- Minimal API
- API REST
- Programación Distribuida
- Comunicación mediante Sockets
- Programación Asincrónica
- Manejo de Excepciones
- Buenas prácticas de desarrollo

---

# Autores

Eric Aguirre y Celedonio Leon Flores

Escuela Técnica N.º 12 D.E. 1

Materia: Programación Sobre Redes

Docente: Luis Durán