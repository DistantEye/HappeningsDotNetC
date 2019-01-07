using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using HappeningsDotNetC.Interfaces.EntityInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HappeningsDotNetC.Controllers
{
    [Authorize]
    public abstract class AppController<TDto> : Controller where TDto : class, IDbDto
    {
        // frequently used calls should be short so long as they're still recognizable 
        protected readonly ILoginService loginService;
        protected readonly IApiService<TDto> apiService;
        protected readonly IApiService<ReminderDto> reminderService;

        // while a Controller can have more than one IApiService, the one designated for apiService should be the one most commonly used
        public AppController(ILoginService lS, IApiService<TDto> aS, IApiService<ReminderDto> reminderServ) : base()
        {
            loginService = lS;
            apiService = aS;
            reminderService = reminderServ;
        }

        // a series of methods to make basic WebApi access also functional
        [Route("/api/[controller]/")]
        public virtual IEnumerable<TDto> ApiGet()
        {
            return apiService.Get();
        }

        [Route("/api/[controller]/getforuser/{id?}")]
        public virtual IEnumerable<TDto> ApiGetForUser(Guid id)
        {
            return apiService.GetForUser(id);
        }

        [Route("/api/[controller]/{id?}")]
        public virtual TDto ApiGet(Guid id)
        {
            return apiService.Get(id);
        }

        [HttpPost("/api/[controller]/")]
        public virtual TDto ApiCreate(TDto dto)
        {
            return apiService.Create(dto);
        }

        [HttpPut("/api/[controller]/")]
        public virtual TDto ApiUpdate(TDto dto)
        {
            return apiService.Update(dto);
        }
      
        [HttpDelete("/api/[controller]/delete/{id}")]
        public virtual IActionResult ApiDelete(Guid id)
        {
            apiService.Delete(id);
            return Ok();
        }

        [HttpPost("/api/[controller]/")]
        public virtual IEnumerable<TDto> ApiCreate(IEnumerable<TDto> dtos)
        {
            return apiService.Create(dtos);
        }

        [HttpPut("/api/[controller]/")]
        public virtual IEnumerable<TDto> ApiUpdate(IEnumerable<TDto> dtos)
        {
            return apiService.Update(dtos);
        }

        [HttpDelete("/api/[controller]/delete/{id}")]
        public virtual IActionResult ApiDelete(IEnumerable<Guid> ids)
        {
            apiService.Delete(ids);
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
