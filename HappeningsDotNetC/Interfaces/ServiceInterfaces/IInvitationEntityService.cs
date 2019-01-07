using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Models.JoinEntities;
using HappeningsDotNetC.Interfaces.EntityInterfaces;
using HappeningsDotNetC.Dtos.IntermediaryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Interfaces.ServiceInterfaces
{
    public interface IInvitationEntityService : IApiEntityService<HappeningUser, InvitationDto>
    {
        HappeningUser GetEnt(Guid userId, Guid happeningId); // should be one or null result
    }
}
