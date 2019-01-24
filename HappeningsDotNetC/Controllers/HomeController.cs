using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using HappeningsDotNetC.Dtos.EntityDtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HappeningsDotNetC.Helpers;
using HappeningsDotNetC.Dtos.IntermediaryDtos;

namespace HappeningsDotNetC.Controllers
{
    public class HomeController : AppController<UserDto>
    {
        private IApiService<InvitationDto> membershipService;

        public HomeController(ILoginService loginService, IApiService<UserDto> apiService, 
                            IApiService<ReminderDto> reminderServ, IApiService<InvitationDto> joinService) : base(loginService, apiService, reminderServ)
        {
            membershipService = joinService;
        }


        public IActionResult Profile(string message = "")
        {
            ViewData["Title"] = "Update Profile:";
            ViewData["Message"] = message;

            return View(apiService.Get(loginService.GetCurrentUserId()));
        }

        [HttpPost]
        public IActionResult UpdateProfile(UserDto dto)
        {
            string messages = "";
            
            if (ModelState.IsValid)
            {
                ApiUpdate(dto);
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).SelectMany(x => x.Key + ": " + String.Join(", ", x.Value.Errors));
                messages = String.Join("; ", errors);
            }

            // success or fail, we go back to the original page
            RedirectToActionResult redirectResult = new RedirectToActionResult("Profile", "Home", new { message = messages });
            return redirectResult;
        }

        public IActionResult Index()
        {
            // placeholder : should redirect to CalendarController which is the actual 'Home'
            RedirectToActionResult redirectResult = new RedirectToActionResult("Index", "Calendar", new { });
            return redirectResult;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // special override for ApiUpdate to bake in the password changing parts 
        public override UserDto ApiUpdate([FromBody] UserDto dto)
        {
            if (String.IsNullOrEmpty(dto.PasswordOrHash))
            {
                return base.ApiUpdate(dto); // if no password is being changed we don't have to go through the extra stuff below
            }

            CreatedLoginDto hashedPwdInfo = loginService.RegisterOrUpdate(new LoginDto() { UserName = dto.Name, Password = dto.PasswordOrHash });

            return base.ApiUpdate(dto.CloneWithNewInfo(hashedPwdInfo));
        }

        // override aspects this endpoint shouldn't be able to act on

        public override UserDto ApiCreate([FromBody] UserDto dto)
        {
            throw new HandledException(new NotImplementedException("Cannot create new users from this endpoint"));
        }

        public override IEnumerable<UserDto> ApiCreate([FromBody] IEnumerable<UserDto> dtos)
        {
            throw new HandledException(new NotImplementedException("Cannot create new users from this endpoint"));
        }

        public override IActionResult ApiDelete(Guid id)
        {
            if (id != loginService.GetCurrentUserId())
            {
                throw new HandledException(new NotImplementedException("Cannot delete other users from this endpoint"));
            }
            else
            {
                var memberships = membershipService.GetForUser(id).Select(x => x.Id);
                membershipService.Delete(memberships, false);
                return base.ApiDelete(id);
            }
        }

        public override IActionResult ApiDelete([FromBody] IEnumerable<Guid> ids)
        {
            throw new HandledException(new NotImplementedException("Cannot mass delete users from this endpoint"));
        }

        public override IEnumerable<UserDto> ApiUpdate([FromBody] IEnumerable<UserDto> dtos)
        {
            throw new HandledException(new NotImplementedException("Cannot mass update users from this endpoint"));
        }
    }
}
