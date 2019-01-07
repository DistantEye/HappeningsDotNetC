using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Models.JoinEntities;
using HappeningsDotNetC.Infrastructure;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Services
{
    public class InvitationService : AbstractApiService<HappeningUser, InvitationDto>, IInvitationEntityService
    {

        private readonly IApiEntityService<Reminder, ReminderDto> reminderService;

        public InvitationService(HappeningsContext hc, ILoginService loginServ, IApiEntityService<Reminder, ReminderDto> reminderServ) : base(hc, loginServ)
        {
            reminderService = reminderServ;
        }

        public override IQueryable<HappeningUser> GetQueryable()
        {
            return base.GetQueryable()
                        .Include(x => x.Happening)
                        .Include(x => x.User);
        }

        public HappeningUser GetEnt(Guid userId, Guid happeningId)
        {
            return GetQueryable().SingleOrDefault(x => x.UserId == userId && x.HappeningId == happeningId);
        }

        public override IEnumerable<InvitationDto> GetForUser(Guid id)
        {
            return GetQueryable().Where(x => x.UserId == id).Select(x => DtoFromEntity(x));
        }

        public override InvitationDto Create(InvitationDto dto, bool commitImmediately = true)
        {
            throw new NotImplementedException("Direct creations of HappeningUsers are not allowed");
        }

        public override IEnumerable<InvitationDto> Create(IEnumerable<InvitationDto> dtos, bool commitImmediately = true)
        {
            throw new NotImplementedException("Direct creations of HappeningUsers are not allowed");
        }

        // Entities can still be made via derived services
        public override HappeningUser CreateEntity(InvitationDto dto)
        {
            var result = new HappeningUser(Guid.NewGuid(), dto.HappeningId, dto.UserId, Enum.Parse<RSVP>(dto.Status, true), dto.ReminderXMinsBefore, dto.IsPrivate);

            // exception to the usual 'One Action per CreateEntity' trend : reminders are tightly bound sub-entities.
            // they could even be derived entities rather than concrete but there's performance benefits of explicitness
            var reminder = new Reminder(Guid.NewGuid(), dto.Date.AddMinutes(result.ReminderXMinsBefore * -1), dto.Date, false, dto.UserId);
            result.SetReminder(reminder);

            return result;
        }

        protected override void UpdateEntity(HappeningUser entity, InvitationDto dto)
        {
            entity.Update(Enum.Parse<RSVP>(dto.Status, true), dto.ReminderXMinsBefore, dto.IsPrivate);

            
            Reminder reminder = entity.Reminder;

            // fetch if necessary
            if (reminder == null)
            {
                reminder = reminderService.GetEnt(entity.ReminderId.Value);
            }

            reminder.Update(dto.Date.AddMinutes(entity.ReminderXMinsBefore * -1), dto.Date, reminder.IsSilenced);
        }

        public override InvitationDto DtoFromEntity(HappeningUser entity)
        {

            return new InvitationDto()
            {
                Id = entity.Id,
                Date = entity.Happening.StartTime,
                HappeningId = entity.HappeningId,
                HappeningName = entity.Happening.Name,
                ReminderXMinsBefore = entity.ReminderXMinsBefore,
                Status = entity.UserStatus.ToString(),
                UserId = entity.UserId,       
                UserName = entity.User != null ? entity.User.FriendlyName : ""
            };
        }
        
    }
}
