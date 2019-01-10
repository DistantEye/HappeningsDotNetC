using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Helpers
{
    // stub class used to catch when exceptions were expected gracefully vs not
    public class HandledException : Exception
    {
        // default case to wrap exceptions
        public HandledException(Exception e) : base(e.Message, e)
        {
        }
        
    }
}
