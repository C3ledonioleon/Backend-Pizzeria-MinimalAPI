CREATE DATABASE IF NOT EXISTS PizzeriaDB;
USE PizzeriaDB;


-- =========================
-- TABLA CLIENTES
-- =========================
CREATE TABLE Clientes (
    IdCliente INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    Direccion VARCHAR(200) NOT NULL,
    Telefono VARCHAR(30) NOT NULL
);


-- =========================
-- TABLA EMPLEADOS
-- =========================
CREATE TABLE Empleados (
    IdEmpleado INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Rol ENUM(
        'Administrador',
        'Cocinero',
        'Repartidor',
    ) NOT NULL,
    DNI INT NOT NULL UNIQUE,
    Telefono VARCHAR(30) NOT NULL
);


-- =========================
-- TABLA SUCURSALES
-- =========================
CREATE TABLE Sucursales (
    IdSucursal INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Direccion VARCHAR(200) NOT NULL,
    Telefono VARCHAR(30) NOT NULL
);


-- =========================
-- TABLA PIZZAS
-- =========================
CREATE TABLE Pizzas (
    IdPizza INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Descripcion TEXT
);


-- =========================
-- TABLA INGREDIENTES
-- (para guardar List<string> Ingredientes)
-- =========================
CREATE TABLE Ingredientes (
    IdIngrediente INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);


-- Relación muchos a muchos Pizza - Ingrediente
CREATE TABLE PizzaIngredientes (
    IdPizza INT NOT NULL,
    IdIngrediente INT NOT NULL,

    PRIMARY KEY(IdPizza, IdIngrediente),

    FOREIGN KEY(IdPizza)
        REFERENCES Pizzas(IdPizza)
        ON DELETE CASCADE,

    FOREIGN KEY(IdIngrediente)
        REFERENCES Ingredientes(IdIngrediente)
        ON DELETE CASCADE
);


-- =========================
-- TABLA PEDIDOS
-- =========================
CREATE TABLE Pedidos (
    IdPedido INT AUTO_INCREMENT PRIMARY KEY,

    FechaHora DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    Estado ENUM(
        'EsperaConfirmacion',
        'EnPreparacion',
        'EnViaje',
        'Entregado'
    ) NOT NULL,

    Total DECIMAL(10,2) NOT NULL DEFAULT 0,

    Direccion VARCHAR(200),

    IdCliente INT NOT NULL,
    IdEmpleado INT NOT NULL,
    IdSucursal INT NOT NULL,


    FOREIGN KEY(IdCliente)
        REFERENCES Clientes(IdCliente),

    FOREIGN KEY(IdEmpleado)
        REFERENCES Empleados(IdEmpleado),

    FOREIGN KEY(IdSucursal)
        REFERENCES Sucursales(IdSucursal)
);


-- =========================
-- TABLA DETALLE PEDIDO
-- =========================
CREATE TABLE DetallePedidos (

    IdDetallePedido INT AUTO_INCREMENT PRIMARY KEY,

    IdPedido INT NOT NULL,

    IdPizza INT NOT NULL,

    Cantidad INT NOT NULL,

    PrecioUnitario DECIMAL(10,2) NOT NULL,

    Observaciones VARCHAR(255),


    FOREIGN KEY(IdPedido)
        REFERENCES Pedidos(IdPedido)
        ON DELETE CASCADE,


    FOREIGN KEY(IdPizza)
        REFERENCES Pizzas(IdPizza)
);