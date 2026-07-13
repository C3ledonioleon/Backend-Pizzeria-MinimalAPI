using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pizzeria.API.Models
{
    public class Sucursal
    {
        public int IdSucursal { get; set; }

        public string Nombre { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }


        public Sucursal (string nombre, string direccion, string telefono)
        {
            Nombre = nombre ;
            Direccion = direccion ; 
            Telefono = telefono ;
        }

    }
}