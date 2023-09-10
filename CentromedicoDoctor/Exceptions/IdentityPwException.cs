using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Exceptions
{
    public class IdentityPwException : Exception
    {
        public IdentityPwException(string message) : base(message)
        {
        }


    }
}
