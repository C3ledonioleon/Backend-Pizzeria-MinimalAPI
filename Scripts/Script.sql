DROP DATABASE IF EXISTS `6to_Pizzeria`;
CREATE DATABASE `6to_Pizzeria`;
USE `6to_Pizzeria`;

CREATE TABLE Usuario (
    IdUsuario INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    Password VARCHAR(256) NOT NULL,
    Rol INT NOT NULL
);

INSERT INTO Usuario (Username, Email, Password, Rol)
VALUES ('admin', 'admin@gmail.com', '3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121', 1);



-- =========================
-- CLIENTES
-- =========================

CREATE TABLE Cliente (
    IdCliente INT AUTO_INCREMENT PRIMARY KEY,
    IdUsuario INT NOT NULL UNIQUE,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    Telefono VARCHAR(30) NOT NULL,
    Direccion VARCHAR(200) NOT NULL,

    CONSTRAINT FK_Cliente_Usuario
        FOREIGN KEY (IdUsuario)
        REFERENCES Usuario(IdUsuario)
);


-- =========================
-- PIZZAS
-- =========================
CREATE TABLE Pizza (
    IdPizza INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Descripcion VARCHAR(300),
    Ingredientes VARCHAR(500)
);

-- =========================
-- PEDIDOS
-- Estado:
-- 1 = EsperaConfirmacion
-- 2 = EnPreparacion
-- 3 = EnViaje
-- 4 = Entregado
-- 5 = Cancelado
-- =========================
CREATE TABLE Pedido (
    IdPedido INT AUTO_INCREMENT PRIMARY KEY,
    FechaHora DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Estado TINYINT NOT NULL,

    IdCliente INT NOT NULL,

    Total DECIMAL(10,2) NOT NULL,

    CONSTRAINT FK_Pedido_Cliente
        FOREIGN KEY (IdCliente)
        REFERENCES Cliente(IdCliente)
);

-- =========================
-- DETALLE DEL PEDIDO
-- =========================
CREATE TABLE DetallePedido (
    IdDetallePedido INT AUTO_INCREMENT PRIMARY KEY,

    IdPedido INT NOT NULL,
    IdPizza INT NOT NULL,

    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Observaciones VARCHAR(200),

    CONSTRAINT FK_DetallePedido_Pedido
        FOREIGN KEY (IdPedido)
        REFERENCES Pedido(IdPedido),

    CONSTRAINT FK_DetallePedido_Pizza
        FOREIGN KEY (IdPizza)
        REFERENCES Pizza(IdPizza)
);

