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

        // in this class we implement more intelligent mass Create, Update, Delete functionality that would logically move towards abstract ApiService, but for right now UserService
        // is the only service with validation concerns that need this advanced behavior, and that's not likely to change in the current application scope. If it does, it should 
        // be backported to AbstractApiService
        public UserService(HappeningsContext hc, ILoginService loginServ) : base(hc, loginServ)
        {

        }

        public override IQueryable<User> GetQueryable(bool local = false)
        {
            return base.GetQueryable(local)
                        .Include(x => x.Happenings).ThenInclude(x => x.Happening);
        }               

        protected virtual void ValidateUser(UserDto dto)
        {
            ValidateUser( ToValidateSet(dto) );
        }

        protected virtual void ValidateUser(IEnumerable<UserValidationDto> modSet)
        {
            int userCount = GetCount();
            int modSetSize = modSet.Count();
            var currentUser = loginService.GetCurrentUser();
            var currentUserRole = currentUser != null ? currentUser.Role : UserRole.Anonymous;
            Guid currentUserId = currentUser != null ? currentUser.Id : Guid.Empty;

            // handle some permissions across the entire set : note that if the Users Table is empty, you can always create an admin user
            bool canCreate = SystemData.OpenRegistration || currentUserRole == UserRole.Admin || (userCount == 0 && modSetSize == 1);
            bool canCreateAdmins = currentUserRole == UserRole.Admin || (userCount == 0 && modSetSize == 1);
            bool canEditOthers = currentUserRole == UserRole.Admin;


            IEnumerable<Guid> modSetIds = modSet.Where(x => x.Id != null && x.Id != Guid.Empty).Select(x => x.Id).ToArray();

            // cast Query to array since we may be making multiple passes on it and EF doesn't always treat this as performance sanely as you'd expect
            IEnumerable<UserValidationDto> data = GetQueryable().Where(x => !modSetIds.Contains(x.Id)).Select(x => new UserValidationDto(x.Id, x.Name, x.Role)).ToArray();

            foreach (UserValidationDto dto in modSet)
            {
                // basic permissions checking
                bool isCreating = dto.Id == Guid.Empty;

                if (isCreating)
                {
                    if (!canCreate)
                    {
                        throw new HandledException(new ArgumentException("User lacks permissions to create new users!"));
                    }
                    if (dto.Role == UserRole.Admin && !canCreateAdmins)
                    {
                        throw new HandledException(new ArgumentException("User lacks permissions to create admin users!"));
                    }
                }
                else
                {
                    bool isOtherUser = dto.Id != currentUserId;

                    // is updating
                    if (isOtherUser && !canEditOthers)
                    {
                        throw new HandledException(new ArgumentException("User lacks permissions to edit other users!"));
                    }
                    if (dto.Role == UserRole.Admin && !canCreateAdmins)
                    {
                        throw new HandledException(new ArgumentException("User lacks permissions to make users admins!"));
                    }
                }

                IEnumerable<UserValidationDto> usersWithName = data.Where(x => x.Name == dto.Name);

                if (usersWithName.Count() > 0)
                {
                    throw new HandledException(new ArgumentException("Name already exists and must be unique: " + dto.Name));
                }
            }
        }

        protected virtual void ValidateAdminStatus(UserDto dto, UserDto excludeDto = null)
        {
            var excludeSet = excludeDto == null ? new List<UserValidationDto>() : new List<UserValidationDto>() { new UserValidationDto(excludeDto.Id, excludeDto.Name, Enum.Parse<UserRole>(excludeDto.Role, true)) };
            ValidateAdminStatus(new List<UserValidationDto>() { new UserValidationDto(dto.Id, dto.Name, Enum.Parse<UserRole>(dto.Role, true)) }, excludeSet);
        }

        // The general idea of a modSet and an excludeSet allows us to use the same method for update and delete
        // modSet provides new data to consider (stuff that's been changed), excludeSet tells us data to ignore (stuff that's been entirely removed)
        protected virtual void ValidateAdminStatus(IEnumerable<UserValidationDto> modSet, IEnumerable<UserValidationDto> excludeSet)
        {
            IEnumerable<Guid> excludeSetIds = excludeSet.Where(x => x.Id != null && x.Id != Guid.Empty).Select(x => x.Id).ToArray();

            // cast Query to array since we may be making multiple passes on it and EF doesn't always treat this as performance sanely as you'd expect
            IEnumerable<UserValidationDto> data = GetQueryable().Where(x => !excludeSetIds.Contains(x.Id)).Select(x => new UserValidationDto(x.Id, x.Name, x.Role)).ToArray();

            // make sure there's still at least one admin user after updates
            var allRemaningUsers = data.Concat(modSet);
            var allRemainingAdminUsers = allRemaningUsers.Where(x => x.Role == UserRole.Admin);

            if (allRemainingAdminUsers.Count() == 0 && allRemaningUsers.Count() > 0)
            {
                throw new HandledException(new ArgumentException("Changes would result in no admin users remaining, there must always be one admin unless it's the last user being deleted"));
            }
        }

        protected override void PreCreate(UserDto dto)
        {
            base.PreCreate(dto);

            ValidateUser(dto);
        }

        protected virtual void PreMassCreate(IEnumerable<UserDto> dtos)
        {
            foreach(UserDto dto in dtos) { base.PreCreate(dto); }

            ValidateUser( ToValidateSet(dtos) );
        }

        protected override void PreUpdate(UserDto dto)
        {
            base.PreUpdate(dto);

            ValidateUser(dto);

            ValidateAdminStatus(dto, dto);            
        }


        protected virtual void PreMassUpdate(IEnumerable<UserDto> dtos)
        {
            foreach (UserDto dto in dtos) { base.PreUpdate(dto); }

            var validateSet = ToValidateSet(dtos);

            ValidateUser( validateSet );

            ValidateAdminStatus(validateSet, validateSet);            
        }

        protected override void PreDelete(User ent)
        {
            base.PreDelete(ent);

            var validateSet = ToValidateSet(ent);

            ValidateAdminStatus(new List<UserValidationDto>(), validateSet); // Deletes only exclude, they don't provide new data (a modset)
        }

        protected virtual void PreMassDelete(IEnumerable<Guid> ids)
        {
            IEnumerable<User> ents = GetQueryable().Where(x => ids.Contains(x.Id));

            foreach (User ent in ents) { base.PreDelete(ent); }

            var validateSet = ToValidateSet(ents);

            ValidateAdminStatus(new List<UserValidationDto>(), validateSet); // Deletes only exclude, they don't provide new data (a modset)
        }


        public override IEnumerable<UserDto> Create(IEnumerable<UserDto> dtos, bool commitImmediately = true)
        {
            PreMassCreate(dtos);

            var result = new List<UserDto>();

            foreach(UserDto dto in dtos) { result.Add(CreateCore(dto, false, true)); }

            if (commitImmediately)
            {
                SaveChanges();
            }

            return result;
        }

        public override IEnumerable<UserDto> Update(IEnumerable<UserDto> dtos, bool commitImmediately = true)
        {
            PreMassUpdate(dtos);

            var result = new List<UserDto>();

            foreach (UserDto dto in dtos) { result.Add(UpdateCore(dto, false, true)); }

            if (commitImmediately)
            {
                SaveChanges();
            }

            return result;
        }

        public override void Delete(IEnumerable<Guid> ids, bool commitImmediately = true)
        {
            PreMassDelete(ids);

            foreach (Guid id in ids) { DeleteCore(id, false, true); }

            if (commitImmediately)
            {
                SaveChanges();
            }
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
            entity.Update(Enum.Parse<UserRole>(dto.Role, true), dto.Name, dto.FriendlyName, dto.CalendarVisibleToOthers, dto.PasswordOrHash);
        }

        // used only internally as a more friendly alternative to Tuple
        protected class UserValidationDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public UserRole Role { get; set; }

            public UserValidationDto(Guid id, string name, UserRole role)
            {
                Id = id;
                Name = name;
                Role = role;
            }
        }

        // various transforms
        protected virtual IEnumerable<UserValidationDto> ToValidateSet(UserDto dto)
        {
            return new List<UserValidationDto>() { new UserValidationDto(dto.Id, dto.Name, Enum.Parse<UserRole>(dto.Role, true)) };
        }

        protected virtual IEnumerable<UserValidationDto> ToValidateSet(IEnumerable<UserDto> dtos)
        {
            return dtos.Select(x => new UserValidationDto(x.Id, x.Name, Enum.Parse<UserRole>(x.Role, true))).ToArray();
        }

        protected virtual IEnumerable<UserValidationDto> ToValidateSet(User ent)
        {
            return new List<UserValidationDto>() { new UserValidationDto(ent.Id, ent.Name, ent.Role) };
        }

        protected virtual IEnumerable<UserValidationDto> ToValidateSet(IEnumerable<User> ents)
        {
            return ents.Select(x => new UserValidationDto(x.Id, x.Name, x.Role)).ToArray();
        }
    }
}
