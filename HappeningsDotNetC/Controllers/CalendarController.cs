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
using HappeningsDotNetC.Helpers;

namespace HappeningsDotNetC.Controllers
{
    public class CalendarController : AppController<HappeningDto>
    {
        private readonly IApiService<UserDto> userService;
        private readonly IHappeningService happeningService;

        public CalendarController(ILoginService loginService, IHappeningService apiService, 
                                    IApiService<UserDto> userServ, IApiService<ReminderDto> reminderServ) : base(loginService, apiService, reminderServ)
        {
            userService = userServ;
            happeningService = apiService;
        }

        
        // also move Entity folder to Model folder for standardization?

        [Route("/api/[controller]/getFiltered")]
        public IEnumerable<IEnumerable<HappeningDto>> ApiGetFiltered(bool isCalendarMode, Guid? userId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            Guid? filterUserId = userId ?? loginService.GetCurrentUserId();

            if (startDate != null && endDate != null && endDate.Value < startDate.Value)
            {
                throw new HandledException(new ArgumentException("EndDate cannot be before StartDate!"));
            }

            CalendarSearchFilterDto searchFilter;
            
            if (isCalendarMode)
            {
                DateTime startDateFilter = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDateFilter = endDate ?? startDateFilter.AddMonths(1).AddDays(-1); // gets around figuring out what the last day of the month is
                searchFilter = new CalendarSearchFilterDto()
                {
                    TextualDisplay = false,
                    UserId = userId ?? loginService.GetCurrentUserId(),
                    StartDate = startDateFilter,
                    EndDate = endDateFilter
                };
            }
            else
            {
                searchFilter = new CalendarSearchFilterDto()
                {
                    TextualDisplay = true,
                    UserId = userId,  // list view allows for searching without a User Filter
                    StartDate = startDate, // there's no point in saying 'start/endDate ?? null'
                    EndDate = endDate 
                };
            }
            

            return happeningService.Get(searchFilter);
        }

        public IActionResult Index(Guid? userId = null, DateTime? startDate = null, DateTime? endDate = null, int? offset = null)
        {
            ViewData["WideForm"] = true;

            
            

            DateTime startDateFilter = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDateFilter = endDate ?? startDateFilter.AddMonths(1).AddDays(-1); // gets around figuring out what the last day of the month is

            if (offset != null)
            {
                startDateFilter = startDateFilter.AddMonths(offset.Value);
                endDateFilter = endDateFilter.AddMonths(offset.Value);

                // redirect to avoid odd behavior with querystring
                return new RedirectToActionResult("Index", "Calendar", new { UserId = userId, StartDate = startDateFilter, EndDate = endDateFilter });
            }

            string monthName = startDateFilter.ToString("MMMM");
            ViewData["MonthName"] = monthName;
            ViewData["Title"] = "Calendar - " + monthName;


            Guid currentUserId = loginService.GetCurrentUserId();

            ViewData["OtherUserName"] = null;
            if (userId != null && userId.Value != currentUserId)
            {
                ViewData["OtherUserName"] = userService.Get(userId.Value).FriendlyName;
            }

            // some of this stuff reacts poorly to being read via QueryString in view so we avoid it
            ViewData["UserId"] = userId ?? loginService.GetCurrentUserId();
            ViewData["StartDate"] = startDateFilter;
            ViewData["EndDate"] = endDateFilter;
            ViewData["UserDropDown"] = happeningService.GetUserDictionary(true, false);

            ViewData["LastDayOfMonth"] = GetEndOfMonth(startDateFilter).Day;
            ViewData["MonthStartOffset"] = (int)GetBeginningOfMonth(startDateFilter).DayOfWeek; // lets us adjust when weeks start/end based on what day the month started at

            IEnumerable<IEnumerable<HappeningDto>> data;
            try
            {
                data = ApiGetFiltered(true, userId, startDateFilter, endDateFilter);
            }
            catch(ArgumentException ae)
            {
                ViewData["Error"] = ae.Message;
                return View();
            }

            return View(data);
        }

        public IActionResult IndexListView(Guid? userId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            ViewData["Title"] = "Happenings View";

            // some of this stuff reacts poorly to being read via QueryString in view so we avoid it
            ViewData["UserId"] = userId ?? Guid.Empty;
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;

            IEnumerable<IEnumerable<HappeningDto>> data;
            ViewData["UserDropDown"] = happeningService.GetUserDictionary(true, true);
            if (userId != null && userId == Guid.Empty)
            {
                userId = null; // it was either this or make a nullable dictionary which seems excessive
            }

            try
            {
                data = ApiGetFiltered(false, userId, startDate, endDate);
            }
            catch (ArgumentException ae)
            {
                ViewData["Error"] = ae.Message;
                return View();
            }

            return View(data.First());
        }

        public IActionResult ViewOther()
        {
            ViewData["Pick User to View"] = "Happenings View";
            ViewData["UserDropDown"] = happeningService.GetUserDictionary(true);
            return View();
        }

        protected DateTime GetBeginningOfMonth(DateTime input)
        {
            return new DateTime(input.Year, input.Month, 1);
        }

        protected DateTime GetEndOfMonth(DateTime input)
        {
            DateTime beginningOfMonth = GetBeginningOfMonth(input);
            return beginningOfMonth.AddMonths(1).AddDays(-1);
        }
    }
}
