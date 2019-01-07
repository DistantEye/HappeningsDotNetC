using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Infrastructure;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Services
{
    public class ReminderService : AbstractApiService<Reminder, ReminderDto>
    {
        // should not have other ApiServices as members since this service is itself sometimes a ApiService member
        public ReminderService(HappeningsContext hc, ILoginService loginServ) : base(hc, loginServ)
        {

        }

        public override IQueryable<Reminder> GetQueryable()
        {
            return base.GetQueryable().Include(x => x.HappeningUser).ThenInclude(x => x.Happening);
        }

        public override IEnumerable<ReminderDto> GetForUser(Guid id)
        {
            return GetQueryable().Where(x => x.HappeningUser.UserId == id).Select(x => DtoFromEntity(x));
        }

        public override Reminder CreateEntity(ReminderDto dto)
        {
            return new Reminder(Guid.NewGuid(), dto.StartRemindAt, dto.HappeningTime, dto.IsSilenced, dto.UserId);
        }

        public override ReminderDto DtoFromEntity(Reminder entity)
        {

            return new ReminderDto()
            {
                Id = entity.Id,
                StartRemindAt = entity.StartRemindAt,
                HappeningTime = entity.HappeningTime,
                HappeningId = entity.HappeningUser.HappeningId,
                HappeningName = entity.HappeningUser.Happening.Name,
                IsSilenced = entity.IsSilenced,
                UserId = entity.HappeningUser.UserId
            };
        }

        protected override void UpdateEntity(Reminder entity, ReminderDto dto)
        {
            entity.Update(dto.StartRemindAt, dto.HappeningTime, dto.IsSilenced);
        }
    }
}
