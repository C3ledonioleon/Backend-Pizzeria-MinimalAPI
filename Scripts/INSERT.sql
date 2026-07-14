USE 6to_Pizzeria;

-- =========================
-- CLIENTES
-- =========================

INSERT INTO Cliente (Nombre, Apellido, Email, Telefono, Direccion)
VALUES
('Juan', 'Perez', 'juan@gmail.com', '1122334455', 'Av. San Martin 123'),
('Maria', 'Gomez', 'maria@gmail.com', '1133445566', 'Belgrano 456'),
('Lucas', 'Fernandez', 'lucas@gmail.com', '1144556677', 'Mitre 789'),
('Camila', 'Lopez', 'camila@gmail.com', '1155667788', 'Rivadavia 100'),
('Sofia', 'Martinez', 'sofia@gmail.com', '1166778899', 'Sarmiento 250');

-- =========================
-- PIZZAS

-- =========================

INSERT INTO Pizza (Nombre, Precio, Descripcion, Ingredientes)
VALUES
('Muzzarella', 8500, 'Pizza clásica de muzzarella', 'Muzzarella, Salsa, Orégano'),
('Napolitana', 9800, 'Con tomate y ajo', 'Muzzarella, Tomate, Ajo'),
('Fugazzeta', 10200, 'Con mucha cebolla', 'Muzzarella, Cebolla'),
('Especial', 11500, 'Jamón y morrones', 'Jamón, Morrón, Muzzarella'),
('Calabresa', 11000, 'Con salame', 'Salame, Muzzarella'),
('Cuatro Quesos', 12500, 'Mezcla de quesos', 'Roquefort, Parmesano, Muzzarella, Provolone');

-- =========================
-- PEDIDOS
-- Estado
-- 1 EsperaConfirmacion
-- 2 EnPreparacion
-- 3 EnViaje
-- 4 Entregado
-- 5 Cancelado
-- =========================

INSERT INTO Pedido
(
    Estado,
    IdCliente,
    Total
)
VALUES
(1,1,18300.00),
(2,2,8500.00),
(3,3,22700.00),
(4,4,12500.00),
(5,5,9800.00);

-- =========================
-- DETALLE PEDIDOS
-- =========================

INSERT INTO DetallePedido
(
    IdPedido,
    IdPizza,
    Cantidad,
    PrecioUnitario,
    Observaciones
)
VALUES

(1,1,1,8500,'Sin aceitunas'),
(1,2,1,9800,'Extra queso'),

(2,1,1,8500,NULL),

(3,4,1,11500,NULL),
(3,5,1,11000,'Bien cocida'),

(4,6,1,12500,'Sin orégano'),

(5,2,1,9800,'Cancelar si demora');