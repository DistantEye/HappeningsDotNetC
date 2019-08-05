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
using HappeningsDotNetC.Helpers;

namespace HappeningsDotNetC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : AppController<UserDto>
    {
        private IApiService<InvitationDto> membershipService;
        private IApiService<SystemDataDto> systemService;

        public AdminController(ILoginService loginService, IApiService<UserDto> apiService,
                                IApiService<ReminderDto> reminderServ, IApiService<InvitationDto> joinService,
                                IApiService<SystemDataDto> sysService) : base(loginService, apiService, reminderServ)
        {
            membershipService = joinService;
            systemService = sysService;
        }

        // Idea is to only have one page (tabular) with updates/create/deletes being able to be made in line and then submitted in bulk

        [HttpGet]
        public IActionResult Index(string message = "")
        {
            ViewData["Title"] = "Administration";
            ViewData["Message"] = message;

            var systemData = ApiSystemGet();
            ViewData["SystemDataId"] = systemData.Id;
            ViewData["OpenRegistration"] = systemData.OpenRegistration;

            return View(ApiGet());
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            try
            {
                ApiDelete(id);
            }
            catch (HandledException he)
            {
                string messages = he.Message;
                return new RedirectToActionResult("Index", "Admin", new { message = messages });
            }            

            return new RedirectToActionResult("Index", "Admin", new { });
        }

        [HttpPost]
        public IActionResult UpdateSystemData(SystemDataDto dto)
        {
            try
            {
                ApiSystemUpdate(dto);
            }
            catch (HandledException he)
            {
                string messages = he.Message;
                return new RedirectToActionResult("Index", "Admin", new { message = messages });
            }

            return new RedirectToActionResult("Index", "Admin", new { });
        }

        [HttpPost]
        public IActionResult Update(IEnumerable<UserDto> userDtos)
        {
            string messages = "";

            if (ModelState.IsValid)
            {
                try
                {
                    ApiUpdate(userDtos);
                }
                catch(HandledException he)
                {
                    messages = he.Message;
                    return new RedirectToActionResult("Index", "Admin", new { message = messages });
                }
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
                try
                {
                    ApiCreate(dto);
                }
                catch (HandledException he)
                {
                    messages = he.Message;
                    return new RedirectToActionResult("Index", "Admin", new { message = messages });
                }
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).SelectMany(x => x.Key + ": " + String.Join(", ", x.Value.Errors));
                messages = String.Join("; ", errors);
            }

            return new RedirectToActionResult("Index", "Admin", new { message = messages });
        }

        public override UserDto ApiUpdate([FromBody] UserDto dto)
        {
            if (dto.PasswordOrHash == null || dto.PasswordOrHash.Trim().Length == 0)
            {
                return base.ApiUpdate(dto); // we can't do this as easily for mass, but we can shortcut here to avoid hashing non-password relevant updates
            }

            CreatedLoginDto hashedPwdInfo = loginService.RegisterOrUpdate(new LoginDto() { UserName = dto.Name, Password = dto.PasswordOrHash });

            return base.ApiUpdate(dto.CloneWithNewInfo(hashedPwdInfo));
        }

        public override IEnumerable<UserDto> ApiUpdate([FromBody] IEnumerable<UserDto> dtos)
        {
            var hashedPwds = loginService.RegisterOrUpdate(dtos.Select(x => new LoginDto() { UserName = x.Name, Password = x.PasswordOrHash }))
                                .ToDictionary(x => x.UserName, x => x);

            var alteredSet = dtos.Select(x => (x.PasswordOrHash == null || x.PasswordOrHash.Trim().Length == 0) ? x : x.CloneWithNewInfo(hashedPwds[x.Name]));

            return base.ApiUpdate(alteredSet);
        }

        public override UserDto ApiCreate([FromBody] UserDto dto)
        {
            CreatedLoginDto hashedPwdInfo = loginService.RegisterOrUpdate(new LoginDto() { UserName = dto.Name, Password = dto.PasswordOrHash });

            return base.ApiCreate(dto.CloneWithNewInfo(hashedPwdInfo));
        }

        public override IEnumerable<UserDto> ApiCreate([FromBody] IEnumerable<UserDto> dtos)
        {
            var hashedPwds = loginService.RegisterOrUpdate(dtos.Select(x => new LoginDto() { UserName = x.Name, Password = x.PasswordOrHash }))
                                .ToDictionary(x => x.UserName, x => x);

            var alteredSet = dtos.Select(x => x.CloneWithNewInfo(hashedPwds[x.Name]));

            return base.ApiCreate(alteredSet);
        }

        // Deletes have to try and clear out Happening memberships first
        public override IActionResult ApiDelete(Guid id)
        {
            // this may be unobtainable later so grab it now
            Guid currentUID = loginService.GetCurrentUserId(true).Value;

            var memberships = membershipService.GetForUser(id).Select(x => x.Id);
            membershipService.Delete(memberships, false);
            var result = base.ApiDelete(id);

            if (id == currentUID)
            {
                // trigger a logout instead of the normal behavior if the current user was deleted
                loginService.Logout();
            }

            return result;
        }

        public override IActionResult ApiDelete([FromBody] IEnumerable<Guid> ids)
        {
            // this may be unobtainable later so grab it now
            Guid currentUID = loginService.GetCurrentUserId(true).Value;

            foreach (Guid id in ids)
            {
                var memberships = membershipService.GetForUser(id).Select(x => x.Id);
                membershipService.Delete(memberships, false);
            }
            var result = base.ApiDelete(ids);

            if (ids.Contains(currentUID))
            {
                // trigger a logout instead of the normal behavior if the current user was deleted
                loginService.Logout();
            }

            return result;
        }

        [HttpGet("/api/[controller]/system")]
        public virtual SystemDataDto ApiSystemGet()
        {
            return systemService.Get().First();
        }

        [HttpPut("/api/[controller]/system")]
        public virtual SystemDataDto ApiSystemUpdate([FromBody] SystemDataDto dto)
        {
            return systemService.Update(dto);
        }
    }
}
