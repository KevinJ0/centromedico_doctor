using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Exceptions
{
    public class NoContentException : Exception
    {
        public NoContentException() : base("No hay contenido que mostrar")
        {
        }


    }
}
