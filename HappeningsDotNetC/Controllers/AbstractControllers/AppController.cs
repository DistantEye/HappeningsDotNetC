﻿using System;
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
        [HttpGet("/api/[controller]/")]
        public virtual IEnumerable<TDto> ApiGet()
        {
            return apiService.Get();
        }

        [HttpGet("/api/[controller]/getforuser/{id?}")]
        public virtual IEnumerable<TDto> ApiGetForUser(Guid id)
        {
            return apiService.GetForUser(id);
        }

        [HttpGet("/api/[controller]/{id?}")]
        public virtual TDto ApiGet(Guid id)
        {
            return apiService.Get(id);
        }

        [HttpPost("/api/[controller]/")]
        public virtual TDto ApiCreate([FromBody] TDto dto)
        {
            return apiService.Create(dto);
        }

        [HttpPut("/api/[controller]/")]
        public virtual TDto ApiUpdate([FromBody] TDto dto)
        {
            return apiService.Update(dto);
        }
      
        [HttpDelete("/api/[controller]/{id}")]
        public virtual IActionResult ApiDelete(Guid id)
        {
            apiService.Delete(id);
            return Ok();
        }

        [HttpPost("/api/[controller]/mass")]
        public virtual IEnumerable<TDto> ApiCreate([FromBody] IEnumerable<TDto> dtos)
        {
            return apiService.Create(dtos);
        }

        [HttpPut("/api/[controller]/mass")]
        public virtual IEnumerable<TDto> ApiUpdate([FromBody] IEnumerable<TDto> dtos)
        {
            return apiService.Update(dtos);
        }

        [HttpPost("/api/[controller]/massDelete/{id}")]
        public virtual IActionResult ApiDelete([FromBody] IEnumerable<Guid> ids)
        {
            apiService.Delete(ids);
            return Ok();
        }
        
    }
}
