using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Middleware
{
    public class LoginVerificationMiddleware
    {
        private readonly RequestDelegate _next;

        public LoginVerificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILoginService loginService)
        {
            // if the login isn't sane anymore we force a logout
            if (!loginService.IsLoginSane())
            {
                loginService.Logout();
            }

            await _next.Invoke(context);

            // Clean up.
        }
    }
}
