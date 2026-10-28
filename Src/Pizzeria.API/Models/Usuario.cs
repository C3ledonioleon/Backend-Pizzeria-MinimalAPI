using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pizzeria.API.Models
{
    public class Usuario
    {
        public int IdUsario {get; set;}
        public string Email {get; set;}
        public string Contrasena {get; set;}

        public Usuario(){}

        public Usuario(string email, string contrasena)
        {
            Email = email;
            Contrasena = contrasena;
        }
    }
}