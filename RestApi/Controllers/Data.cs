using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Controllers
{

    public class CharacterModel
    {

        public string Name { get; set; }
        public string Id { get; set; }
        public string Image { get; set; }
        public string Species { get; set; }
        public string Status { get; set; }

        public List<string> episode { get; set; }

    }
}
