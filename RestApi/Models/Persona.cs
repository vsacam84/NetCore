using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{
    public class Persona
    {
        public string id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string status { get; set; }
        public string species { get; set; }
        public int cantApariciones { get; set; }

    }
}
