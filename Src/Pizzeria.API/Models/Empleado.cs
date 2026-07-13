using System;

namespace Pizzeria.API.Models
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public RolEmpleado Rol { get; set; }
        public int DNI { get; set; }
        public string Telefono { get; set; }

        public Empleado(string nombre, string apellido, RolEmpleado rol, int dni, string telefono)
        {
            Nombre = nombre;
            Apellido = apellido;
            Rol = rol;
            DNI = dni;
            Telefono = telefono;
        }
    }
}