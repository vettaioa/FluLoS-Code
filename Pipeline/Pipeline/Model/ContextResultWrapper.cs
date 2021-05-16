using SharedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline.Model
{
    class ContextResultWrapper
    {
        public MessageContext LuisContext { get; set; }
        public MessageContext RmlContext { get; set; }
    }
}
