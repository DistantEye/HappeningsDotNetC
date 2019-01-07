using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HappeningsDotNetC.Dtos.IntermediaryDtos;

namespace HappeningsDotNetC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : AppController<UserDto>
    {
        private IApiService<InvitationDto> membershipService;

        public AdminController(ILoginService loginService, IApiService<UserDto> apiService, 
                                IApiService<ReminderDto> reminderServ, IApiService<InvitationDto> joinService) : base(loginService, apiService, reminderServ)
        {
            membershipService = joinService;
        }

        // Idea is to only have one page (tabular) with updates/create/deletes being able to be made in line and then submitted in bulk

        [HttpGet]
        public IActionResult Index(string message = "")
        {
            ViewData["Title"] = "Administration";
            ViewData["Message"] = message;

            return View(ApiGet());
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            ApiDelete(id);

            return new RedirectToActionResult("Index", "Admin", new { });
        }

        [HttpPost]
        public IActionResult Update(IEnumerable<UserDto> userDtos)
        {
            string messages = "";

            if (ModelState.IsValid)
            {
                ApiUpdate(userDtos);
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).SelectMany(x => x.Key + ": " + String.Join(", ", x.Value.Errors));
                messages = String.Join("; ", errors);
            }            

            return new RedirectToActionResult("Index", "Admin", new { message = messages });
        }

        [HttpPost]
        public IActionResult Create(UserDto dto)
        {
            string messages = "";

            if (ModelState.IsValid)
            {
                ApiCreate(dto);
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).SelectMany(x => x.Key + ": " + String.Join(", ", x.Value.Errors));
                messages = String.Join("; ", errors);
            }

            return new RedirectToActionResult("Index", "Admin", new { message = messages });
        }        

        public override UserDto ApiCreate(UserDto dto)
        {
            CreatedLoginDto hashedPwdInfo = loginService.RegisterOrUpdate(new LoginDto() { UserName = dto.Name, Password = dto.PasswordOrHash });

            return base.ApiCreate(dto.CloneWithNewInfo(hashedPwdInfo));
        }

        public override IEnumerable<UserDto> ApiCreate(IEnumerable<UserDto> dtos)
        {
            var hashedPwds = loginService.RegisterOrUpdate(dtos.Select(x => new LoginDto() { UserName = x.Name, Password = x.PasswordOrHash }))
                                .ToDictionary(x => x.UserName, x => x);

            var alteredSet = dtos.Select(x => x.CloneWithNewInfo(hashedPwds[x.Name]));

            return base.ApiCreate(alteredSet);
        }

        // Deletes have to try and clear out Happening memberships first
        public override IActionResult ApiDelete(Guid id)
        {            
            var memberships = membershipService.GetForUser(id).Select(x => x.Id);
            membershipService.Delete(memberships, false);
            return base.ApiDelete(id);
        }

        public override IActionResult ApiDelete(IEnumerable<Guid> ids)
        {
            foreach(Guid id in ids)
            {
                var memberships = membershipService.GetForUser(id).Select(x => x.Id);
                membershipService.Delete(memberships, false);
            }
            return base.ApiDelete(ids);
        }
    }
}
