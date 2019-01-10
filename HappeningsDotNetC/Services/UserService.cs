using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Infrastructure;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HappeningsDotNetC.Helpers;

namespace HappeningsDotNetC.Services
{
    public class UserService : AbstractApiService<User, UserDto>
    {

        // UserService should not include other AbstractApiServices services since it is a common side include itself
        public UserService(HappeningsContext hc, ILoginService loginServ) : base(hc, loginServ)
        {

        }

        public override IQueryable<User> GetQueryable()
        {
            return base.GetQueryable()
                        .Include(x => x.Happenings).ThenInclude(x => x.Happening);
        }

        protected virtual void ValidateUser(UserDto dto)
        {
            User existingUser = dto.Id == Guid.Empty ? null : GetQueryable().SingleOrDefault(x => x.Id == dto.Id);

            IQueryable<User> usersWithName = GetQueryable().Where(x => x.Name == dto.Name
                                                                                     && (existingUser == null || x.Id != existingUser.Id));
            
            if (usersWithName.Count() > 0)
            {
                throw new HandledException(new ArgumentException("Name already exists and must be unique: " + dto.Name));
            }
        }

        protected override void PreCreate(UserDto dto)
        {
            base.PreCreate(dto);

            ValidateUser(dto);
        }

        protected override void PreUpdate(UserDto dto)
        {
            base.PreUpdate(dto);

            ValidateUser(dto);
        }

        public override User CreateEntity(UserDto dto)
        {
            return new User(Guid.NewGuid(), Enum.Parse<UserRole>(dto.Role, true), dto.Name, dto.FriendlyName, dto.CalendarVisibleToOthers, dto.PasswordOrHash);
        }

        public override UserDto DtoFromEntity(User entity)
        {
            return new UserDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                FriendlyName = entity.Name,
                CalendarVisibleToOthers = entity.CalendarVisibleToOthers,
                Role = entity.Role.ToString(),
                Happenings = entity.Happenings == null ? null : entity.Happenings.Select(x => x.HappeningId)
            };
        }

        protected override void UpdateEntity(User entity, UserDto dto)
        {
            entity.Update(Enum.Parse<UserRole>(dto.Role, true), dto.Name, dto.FriendlyName, dto.CalendarVisibleToOthers);
        }
    }
}
