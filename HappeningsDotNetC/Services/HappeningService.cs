using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Models.JoinEntities;
using HappeningsDotNetC.Infrastructure;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using HappeningsDotNetC.Dtos.IntermediaryDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HappeningsDotNetC.Helpers;

namespace HappeningsDotNetC.Services
{
    public class HappeningService : AbstractApiService<Happening, HappeningDto>, IHappeningService
    {
        // Usually it's a bad idea leading to DI hell to have services like these contain other services, but things like HappeningUser are the exception
        // InvitationService is pretty tightly bound and defined as a sub service and not likely to cause circular dependencies

        private readonly IInvitationEntityService joinService;
        private readonly IApiEntityService<User,UserDto> userService;

        public HappeningService(HappeningsContext hc, ILoginService loginServ, IInvitationEntityService invitationService, IApiEntityService<User, UserDto> userServ) : base(hc, loginServ)
        {
            joinService = invitationService;
            userService = userServ;
        }

        protected override void PreUpdate(HappeningDto dto)
        {
            base.PreUpdate(dto);
            var currentUser = loginService.GetCurrentUser();
            
            if (dto.ControllingUserId != currentUser.Id && currentUser.Role != UserRole.Admin)
            {
                throw new HandledException(new ArgumentException("Non-admin users can only edit an happening if they are the owning user for that happening"));
            }
        }

        public override IQueryable<Happening> GetQueryable()
        {
            return base.GetQueryable()
                        .Include(x => x.ControllingUser)
                        .Include(x => x.AllUsers).ThenInclude(x => x.User);
        }

        public IEnumerable<IEnumerable<HappeningDto>> Get(CalendarSearchFilterDto filter)
        {
            // finding out whether the filter is asking for the logged in user or another user effects permissions
            User currentUser = loginService.GetCurrentUser();

            // although UserId == null is a different case which doesn't need the same direct permissions check            
            bool isCurrentUser = filter.UserId != null && filter.UserId.Value == loginService.GetCurrentUserId();
            
            if (!isCurrentUser && filter.UserId != null)
            {
                User user = userService.GetEnt(filter.UserId.Value);

                // in all cases the filters will restrict private data from being shown, but an error is good feedback for a faulty case
                // without this, they'd just get 0 
                if (!user.CalendarVisibleToOthers && currentUser.Role != UserRole.Admin)
                {
                    throw new HandledException(new ArgumentException("Can't view calendar for user set to private"));
                }
            }

            IEnumerable<HappeningDto> data = GetQueryable().Where(x => (filter.UserId == null || (x.AllUsers.Select(y => y.UserId).Contains(filter.UserId.Value) 
                                                                                                  && (x.AllUsers.FirstOrDefault(y => filter.UserId.Value == y.UserId).User.CalendarVisibleToOthers
                                                                                                      || x.AllUsers.FirstOrDefault(y => filter.UserId.Value == y.UserId).User.Id == currentUser.Id
                                                                                                      || currentUser.Role == UserRole.Admin)
                                                                                                  ))
                                                                           && ((isCurrentUser || currentUser.Role == UserRole.Admin) || !x.IsPrivate)
                                                                           && (filter.StartDate == null || x.StartTime >= filter.StartDate.Value)
                                                                           && (filter.EndDate == null || x.EndTime <= filter.EndDate.Value))
                                            .Select(x => DtoFromEntity(x));

            if (filter.TextualDisplay)
            {
                List<IEnumerable<HappeningDto>> result = new List<IEnumerable<HappeningDto>>();
                result.Add(data);
                return result;
            }
            else
            {
                // if Calendar mode, must have startDate/endDate set
                if (filter.StartDate == null || filter.EndDate == null || filter.StartDate.Value.Month != filter.EndDate.Value.Month)
                {
                    throw new HandledException(new ArgumentException("Start and End Date filters must be set, and be within the same month, when textual display is false"));
                }

                int days = DateTime.DaysInMonth(filter.StartDate.Value.Year, filter.StartDate.Value.Month);                
                
                List<HappeningDto>[] result = new List<HappeningDto>[days];
                // intialize all lists
                for (int x = 0; x < result.Length; x++)
                {
                    result[x] = new List<HappeningDto>();
                }

                // return empty set if no data/rows
                if (data.Count() == 0) return result;

                IEnumerable<HappeningDto> sortedData = data.OrderBy(x => x.StartTime);

                // start and end end range offset by -1 to match array notation

                int startDay = sortedData.First().StartTime.Day - 1;
                int endDay = sortedData.Last().StartTime.Day - 1;

                for (int x = startDay; x <= endDay; x++)
                {
                    // find all happenings that are occuring on at least the particular day
                    IEnumerable<HappeningDto> relevantRows = sortedData.Where(i => x+1 >= i.StartTime.Day && x+1 <= i.EndTime.Day );
                    result[x].AddRange(relevantRows);
                }

                return result;
            }

        }

        public override Happening CreateEntity(HappeningDto dto)
        {
            return new Happening(Guid.NewGuid(), dto.Name, dto.Description, dto.ControllingUserId, dto.StartTime, dto.EndTime, dto.IsPrivate);
        }

        public override HappeningDto DtoFromEntity(Happening entity)
        {

            return new HappeningDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ControllingUserId = entity.ControllingUserId,
                ControllingUser = entity.ControllingUser == null ? null : entity.ControllingUser.Name,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                AllUsers = entity.AllUsers.Select(x => x.Id),
                IsPrivate = entity.IsPrivate               
            };
        }

        protected override void UpdateEntity(Happening entity, HappeningDto dto)
        {
            entity.Update(dto.Name, dto.Description, dto.ControllingUserId, dto.StartTime, dto.EndTime, dto.IsPrivate);
        }

        public IEnumerable<InvitationDto> GetHappeningMembership(Guid? happeningId = null)
        {
            if (happeningId == null)
            {
                if (loginService.GetCurrentUser().Role != UserRole.Admin)
                {
                    throw new HandledException(new ArgumentException("Can't request all membership data without admin rights"));
                }

                return joinService.Get();
            }
            else
            {
                Happening happening = GetEnt(happeningId.Value);
                User currentUser = loginService.GetCurrentUser();
                if (happening.IsPrivate && happening.ControllingUserId != currentUser.Id && currentUser.Role != UserRole.Admin)
                {
                    throw new HandledException(new ArgumentException("Can only view happening membership if it is non-private, the current user is the owner, or is an admin"));
                }

                return joinService.GetQueryable().Where(x => x.HappeningId == happeningId).Select(x => joinService.DtoFromEntity(x));
            }
        }

        public HappeningDto AddUser(Guid happeningId, Guid userId, bool commitImmediately = true)
        {
            // Happening.Name later isn't 100% needed but this serves to make sure the specified Happening actually exists
            Happening happening = GetEnt(happeningId);

            // make sure the user hasn't already been added 
            if (joinService.GetEnt(userId, happeningId) != null) { throw new HandledException(new ArgumentException("Cannot add a user already added to event")); }

            var userToAdd = userService.GetEntOrDefault(userId);

            if (userToAdd == null) { throw new HandledException(new ArgumentException("Cannot add a user that doesn't exist")); }

            // happening has to either be non-private, the logged in user doing the adding has to be the owner, or the logged in user has to be an admin
            User currentUser = loginService.GetCurrentUser();
            if (happening.IsPrivate && happening.ControllingUserId != currentUser.Id && currentUser.Role != UserRole.Admin)
            {
                throw new HandledException(new ArgumentException("Can only add a user to a happening if it is non-private, the current user is the owner, or is an admin"));
            }

            InvitationDto result = new InvitationDto()
            {
                HappeningId = happeningId,
                UserId = userId,
                UserName = userToAdd.FriendlyName,
                ReminderXMinsBefore = 0,
                HappeningName = happening.Name,
                Date = happening.StartTime,
                Status = RSVP.NoResponse.ToString(),
                IsPrivate = happening.IsPrivate
            };

            HappeningUser joinEntity = joinService.CreateEntity(result);
            happening.AllUsers.Add(joinEntity);

            if (commitImmediately)
            {
                SaveChanges();
                return Get(happeningId);
            }
            else
            {
                HappeningDto projectedResult = DtoFromEntity(happening);
                projectedResult.AllUsers = projectedResult.AllUsers.Append(userId);
                return projectedResult;
            }
        }

        public HappeningDto RemoveUser(Guid happeningId, Guid userId, bool commitImmediately = true)
        {
            // make sure the specified Happening actually exists
            Happening happening = GetEnt(happeningId);

            HappeningUser joinEntity = joinService.GetEnt(userId, happeningId);
            if (joinEntity == null) { throw new HandledException(new ArgumentException("Cannot remove a user that isn't attached to the event")); }

            // happening has to either be non-private, the logged in user doing the adding has to be the owner, or the logged in user has to be an admin
            User currentUser = loginService.GetCurrentUser();
            if (userId != currentUser.Id && happening.ControllingUserId != currentUser.Id && currentUser.Role != UserRole.Admin)
            {
                throw new HandledException(new ArgumentException("Can only remove a user from happening if the current user is the one being removed, is the owner, or is an admin"));
            }


            happening.AllUsers.Remove(joinEntity);
            joinService.DeleteEnt(joinEntity);

            if (commitImmediately)
            {
                SaveChanges();
                return Get(happeningId);
            }
            else
            {
                HappeningDto projectedResult = DtoFromEntity(happening);
                projectedResult.AllUsers = projectedResult.AllUsers.Where(x => x != userId);
                return projectedResult;
            }
        }

        public Dictionary<Guid, string> GetUserDictionary(bool respectPermissions = true, bool includeEmptyKey = false)
        {
            var currentUser = loginService.GetCurrentUser();
            Dictionary<Guid, string> result;
            if (respectPermissions && currentUser.Role != UserRole.Admin)
            {
                result = userService.GetQueryable().Where(x => x.Id == currentUser.Id || x.CalendarVisibleToOthers).ToDictionary(x => x.Id, x => x.FriendlyName);
            }
            else
            {
                result = userService.Get().ToDictionary(x => x.Id, x => x.FriendlyName);
            }

            if (includeEmptyKey)
            {
                result[Guid.Empty] = "";
            }

            return result;
        }
    }
}
