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
    public class InvitationController : AppController<InvitationDto>
    {
 
        public InvitationController(ILoginService loginService, IApiService<InvitationDto> apiService, IApiService<ReminderDto> reminderServ) : base(loginService, apiService, reminderServ)
        {
        }

        [HttpGet]
        public IActionResult Index(bool includeResolved = false)
        {
            ViewData["Title"] = "Invitations";

            ViewData["IncludeResolved"] = includeResolved;
            IEnumerable<InvitationDto> data = ApiGetForUser(loginService.GetCurrentUserId()).Where(x => includeResolved ||
                                                                                                (!x.Status.Equals("Yes", StringComparison.InvariantCultureIgnoreCase)
                                                                                                    && !x.Status.Equals("No", StringComparison.InvariantCultureIgnoreCase)));
            return View(data);
        }

        [HttpPost]
        public IActionResult Update(IEnumerable<InvitationDto> toUpdate)
        {
            // run Updates to service and then return back to normal index
            ApiUpdate(toUpdate);
            return new RedirectToActionResult("Index", "Invitation", new { });
        }

    }
}
