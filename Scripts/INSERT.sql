USE `5to_Pizzeria`;


INSERT INTO Usuario (Username, Email, Password, Rol)
VALUES
('juan', 'juan@gmail.com', '$2a$11$9XnZavk4XnpXATcarWO7ZOr8TTJY2OlJ6/G.mEqD7zAAzH9tyfIfO', 2),
('pedro', 'pedro@gmail.com', '$2a$11$S74QQJx7F9ywF92BtcynFufygOpijs7uyt8Ybd11UTHAgXzquxW3K', 2),
('maria', 'maria@gmail.com', '$2a$11$bSPzbeQAswl8rVS2MkKm0.fVn.jNNHP.pe2BlfHRGAurws.5bn3te', 2),
('lucas', 'lucas@gmail.com', '$2a$11$rIzkDscSvhOcXKzRq2JhTOnK1PfVU98EDvkTmju/ZezXA0rbCDJK6', 2),
('ana', 'ana@gmail.com', '$2a$11$jNGlCPewIjN0kFYjmmmJ0enXqueQ7Qo.rIHq9hSHhY6uG6FIX8Hiu', 2);
-- =========================
-- CLIENTES
-- =========================

INSERT INTO Cliente(IdUsuario, Nombre, Apellido, Email, Telefono, Direccion)
VALUES
(2, 'Juan', 'Perez', 'juan@gmail.com', '1122334455', 'Av. San Martin 123'),
(3, 'Pedro', 'Gomez', 'pedro@gmail.com', '1122334456', 'Av. Rivadavia 456'),
(4, 'Maria', 'Lopez', 'maria@gmail.com', '1122334457', 'Av. Corrientes 789'),
(5, 'Lucas', 'Diaz', 'lucas@gmail.com', '1122334458', 'Av. Belgrano 321'),
(6, 'Ana', 'Martinez', 'ana@gmail.com', '1122334459', 'Av. Cabildo 654');

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

INSERT INTO Pedido(Estado, IdCliente, Total)
VALUES
(1,1,18300.00),(2,2,8500.00),(3,3,22700.00),(4,4,12500.00),(5,5,9800.00);

-- =========================
-- DETALLE PEDIDOS
-- =========================

INSERT INTO DetallePedido
(IdPedido, IdPizza, Cantidad, PrecioUnitario, Observaciones)
VALUES
(1,1,1,8500,'Sin aceitunas'),
(1,2,1,9800,'Extra queso'),
(2,1,1,8500,NULL),
(3,4,1,11500,NULL),
(3,5,1,11000,'Bien cocida'),
(4,6,1,12500,'Sin orégano'),
(5,2,1,9800,'Cancelar si demora');