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

namespace HappeningsDotNetC.Controllers
{
    public class HomeController : AppController<UserDto>
    {

        public HomeController(ILoginService loginService, IApiService<UserDto> apiService, IApiService<ReminderDto> reminderServ) : base(loginService, apiService, reminderServ)
        {
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
    }
}
