using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{
    public class ListPersona
    {
        public List<Persona> listaVivos { get; set; }
        public List<Persona> listaMuertos { get; set; }

        public int cantidadV { get; set; }
        public int cantidadM { get; set; }
    }
}
