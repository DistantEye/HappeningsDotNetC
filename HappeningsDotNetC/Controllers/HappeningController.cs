using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using HappeningsDotNetC.Dtos.IntermediaryDtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HappeningsDotNetC.Controllers
{
    public class HappeningController : AppController<HappeningDto>
    {
        private readonly IHappeningService happeningService;
        private readonly IApiService<InvitationDto> invitationService;

        public HappeningController(ILoginService loginService, IHappeningService apiService, 
                                    IApiService<InvitationDto> invitationServ, IApiService<ReminderDto> reminderServ) : base(loginService, apiService,reminderServ)
        {
            happeningService = apiService;
            invitationService = invitationServ;
        }        

        public IActionResult View(Guid id)
        {
            ViewData["Title"] = "View Happening";            

            HappeningDto happening = ApiGet(id);

            var currentUser = loginService.GetCurrentUser();

            if (currentUser.Role == UserRole.Admin || happening.ControllingUserId == currentUser.Id)
            {
                ViewData["IsAdmin"] = true;                
            }
            else
            {
                ViewData["IsAdmin"] = false;
            }

            InvitationDto data = happeningService.GetHappeningMembership(id).SingleOrDefault(x => x.UserId == currentUser.Id);

            // get what little Happening data we need to the View
            ViewData["HappeningName"]              = happening.Name;
            ViewData["HappeningDesc"]              = happening.Description;
            ViewData["HappeningControllingUser"]   = happening.ControllingUser;
            ViewData["HappeningStart"]             = happening.StartTime;
            ViewData["HappeningEnd"]               = happening.EndTime;

            ViewData["HappeningId"] = happening.Id;
            ViewData["UserId"] = loginService.GetCurrentUserId();

            // While View is related in design to edit it won't share the same view since the submit target is different and many fields are readonly

            if (StringValues.IsNullOrEmpty(Request.Headers["Referer"]))
            {
                ViewData["Referer"] = Request.Headers["Referer"].ToString();
            }

            return View(data);
        }
        

        public IActionResult Create(string message = "")
        {
            ViewData["Id"] = null;
            ViewData["Title"] = "Create Happening";

            ViewData["PageVerb"] = "Create";
            ViewData["UserDropDown"] = happeningService.GetUserDictionary();

            HappeningDto data = TempData.Peek("HappeningFormData") != null ? (HappeningDto)JsonConvert.DeserializeObject((string)TempData["HappeningFormData"]) : new HappeningDto();

            if (StringValues.IsNullOrEmpty(Request.Headers["Referer"]))
            {
                ViewData["Referer"] = Request.Headers["Referer"].ToString();
            }

            // Create is just a blank Edit page
            return View("Edit", data);
        }

        [HttpGet]
        public IActionResult Edit(Guid id, string message = "")
        {
            ViewData["Title"] = "Edit Happening";

            ViewData["PageVerb"] = "Edit";
            ViewData["UserDropDown"] = happeningService.GetUserDictionary();

            HappeningDto data = TempData.Peek("HappeningFormData") != null ? (HappeningDto)JsonConvert.DeserializeObject((string)TempData["HappeningFormData"]) : ApiGet(id);
            data.AllUserInfo = happeningService.GetHappeningMembership(data.Id);

            var currentUser = loginService.GetCurrentUser();

            if (data.ControllingUserId != currentUser.Id && currentUser.Role != UserRole.Admin)
            {
                return new RedirectToActionResult("View", "Happening", new { id = id }); // non-owner/non-admins can't edit events, silent switch to view
            }

            if (StringValues.IsNullOrEmpty(Request.Headers["Referer"]))
            {
                ViewData["Referer"] = Request.Headers["Referer"].ToString();
            }

            return View(data);
        }

        // receiving end for Creates and Edits both
        [HttpPost]
        public IActionResult Write(HappeningDto dto)
        {            

            if (dto.Id == null || dto.Id == Guid.Empty )
            {
                if (!ModelState.IsValid)
                {
                    TempData["HappeningFormData"] = JsonConvert.SerializeObject(dto);
                    var errors = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).SelectMany(x => x.Key + ": " + String.Join(", ", x.Value.Errors));
                    return new RedirectToActionResult("Create", "Happening", new { messages = String.Join("; ", errors) }); // go back to edit page when done
                }

                HappeningDto result = ApiCreate(dto);
                return new RedirectToActionResult("Edit", "Happening", new { id = result.Id }); // go back to edit page when done
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    TempData["FormData"] = JsonConvert.SerializeObject(dto);
                    var errors = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).SelectMany(x => x.Key + ": " + String.Join(", ", x.Value.Errors));
                    return new RedirectToActionResult("Edit", "Happening", new { id = dto.Id, messages = String.Join("; ", errors) }); // go back to edit page when done
                }

                ApiUpdate(dto);
                return new RedirectToActionResult("Index", "Calendar", new { });
            }
            
        }

        [HttpPost]
        public IActionResult AddHappeningMember(HappeningMembershipDto dto)
        {
            ApiAddHappeningMember(dto);

            return new RedirectToActionResult("Edit", "Happening", new { id = dto.HappeningId});
        }

        // note only the MVC version needs the Bind prefix to workaround a .NET oddity with nesting Models, the API is fine without it
        // the better solution to this would to be using more javascript heavy forms and quite possibly straight ajax, but as per the readme,
        // those solutions aren't desired to exist in the MVC version of the site

        [HttpPost]
        public IActionResult RemoveHappeningMember([Bind(Prefix = "UserElement")] HappeningMembershipDto dto)
        {
            ApiRemoveHappeningMember(dto);

            return new RedirectToActionResult("Edit", "Happening", new { id = dto.HappeningId });
        }        

        [HttpPost]
        public IActionResult UpdateHappeningMember([Bind(Prefix = "userElement")] InvitationDto dto)
        {
            ApiUpdateHappeningMember(dto);

            return new RedirectToActionResult("Edit", "Happening", new { id = dto.HappeningId });
        }

        // to make matters worse, sometimes we need to use the version without the bind so there has to be a differently named action that pipes into the first set

        [HttpPost]
        public IActionResult RemoveHappeningMemberNB(HappeningMembershipDto dto)
        {
            return RemoveHappeningMember(dto);
        }

        [HttpPost]
        public IActionResult UpdateHappeningMemberNB(InvitationDto dto)
        {
            return UpdateHappeningMember(dto);
        }

        [HttpPost("/api/[controller]/addhappeningmember")]
        public HappeningDto ApiAddHappeningMember(HappeningMembershipDto dto)
        {
            return happeningService.AddUser(dto.HappeningId, dto.UserId);
        }

        [HttpPost("/api/[controller]/addhappeningmember")]
        public IActionResult ApiRemoveHappeningMember(HappeningMembershipDto dto)
        {
            happeningService.RemoveUser(dto.HappeningId, dto.UserId);

            return Ok();
        }

        [HttpPost("/api/[controller]/updatehappeningmember")]
        public InvitationDto ApiUpdateHappeningMember(InvitationDto dto)
        {
            return invitationService.Update(dto);
        }
    }
}
