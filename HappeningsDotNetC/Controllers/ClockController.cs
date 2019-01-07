using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;

namespace HappeningsDotNetC.Controllers
{
    public class ClockController : AppController<ReminderDto>
    {
        public ClockController(ILoginService loginService, IApiService<ReminderDto> apiService, IApiService<ReminderDto> reminderServ) : base(loginService, apiService, reminderServ)
        {
        }

        // will need some kind of javascript periodic pull for the Index page to actually retrieve active reminders

        [HttpGet]
        public IActionResult Index(bool includeSilenced = false, bool showNotYetActive = false)
        {
            ViewData["Title"] = "Reminders";
            ViewData["IncludeSilenced"] = includeSilenced;

            IEnumerable<ReminderDto> userReminders = ApiGetData(includeSilenced, showNotYetActive);
            return View(userReminders);
        }

        // for direct webapi grabs
        [Route("/api/[controller]/getdata")]
        public IEnumerable<ReminderDto> ApiGetData(bool includeSilenced = false, bool showNotYetActive = false)
        {
            return apiService.GetForUser(loginService.GetCurrentUserId()).Where(x => (includeSilenced || !x.IsSilenced) && (showNotYetActive || DateTime.Now >= x.StartRemindAt));
        }

        // for direct webapi grabs
        [Route("/api/[controller]/getcount")]
        public int ApiCount(bool includeSilenced = false, bool showNotYetActive = false)
        {
            return ApiGetData(includeSilenced, showNotYetActive).Count();
        }

        [HttpPost]
        public IActionResult Update(ReminderDto toUpdate)
        {
            // run Updates to service and then return back to normal index
            ApiUpdate(toUpdate);
            return new RedirectToActionResult("Index", "Clock", new { });
        }

    }
}
