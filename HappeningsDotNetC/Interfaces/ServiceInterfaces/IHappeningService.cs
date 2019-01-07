using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Models.JoinEntities;
using HappeningsDotNetC.Interfaces.EntityInterfaces;
using HappeningsDotNetC.Dtos.IntermediaryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Interfaces.ServiceInterfaces
{
    public interface IHappeningService : IApiService<HappeningDto>
    {
        HappeningDto AddUser(Guid happeningId, Guid userId, bool commitImmediately = true);
        HappeningDto RemoveUser(Guid happeningId, Guid userId, bool commitImmediately = true);

        IEnumerable<IEnumerable<HappeningDto>> Get(CalendarSearchFilterDto filter);
        Dictionary<Guid, string> GetUserDictionary(bool respectPermissions = true, bool includeEmptyKey = false);

        IEnumerable<InvitationDto> GetHappeningMembership(Guid? happeningId = null);
    }
}
