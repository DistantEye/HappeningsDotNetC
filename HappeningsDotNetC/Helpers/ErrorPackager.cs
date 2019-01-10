using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Helpers
{
    public static class ErrorPackager
    {        
        public static int GetHttpCode(Exception e)
        {
            Type eType = e.GetType();

            // if not handled is always a 500
            if (eType == typeof(HandledException))
            {
                // InnerException should always be set for HandledException
                eType = e.InnerException.GetType();
                if (eType == typeof(ArgumentException))
                {
                    return (int)HttpStatusCode.NotAcceptable;
                }
                else if (eType == typeof(KeyNotFoundException))
                {
                    return (int)HttpStatusCode.NotFound;
                }
                else if (eType == typeof(NotImplementedException))
                {
                    return (int)HttpStatusCode.MethodNotAllowed;
                }
                else
                {
                    return (int)HttpStatusCode.InternalServerError; // fallback is 500
                }
            }
            else
            {
                return (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
