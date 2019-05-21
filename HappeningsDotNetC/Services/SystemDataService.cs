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
    public class SystemDataService : AbstractApiService<SystemData, SystemDataDto>
    {
        // should not have other ApiServices as members since this service is itself sometimes a ApiService member
        public SystemDataService(HappeningsContext hc, ILoginService loginServ) : base(hc, loginServ)
        {

        }

        // special behavior because of the single row nature of SystemData
        public override IQueryable<SystemData> GetQueryable(bool local = false)
        {
            return new[] { happeningsContext.SystemData }.AsQueryable();
        }


        public override void DeleteEnt(SystemData ent)
        {
            throw new NotImplementedException("SystemData entities can't be removed");
        }

        protected override void AddEnt(SystemData ent)
        {
            throw new NotImplementedException("New SystemData entities can never be created by users");
        }

        public override SystemData CreateEntity(SystemDataDto dto)
        {
            throw new NotImplementedException("New SystemData entities can never be created by users");
        }

        public override SystemDataDto DtoFromEntity(SystemData entity)
        {
            return new SystemDataDto()
            {
                Id = entity.Id,
                OpenRegistration = entity.OpenRegistration
            };
        }

        protected override void UpdateEntity(SystemData entity, SystemDataDto dto)
        {
            entity.Update(dto.OpenRegistration);
        }
    }
}
