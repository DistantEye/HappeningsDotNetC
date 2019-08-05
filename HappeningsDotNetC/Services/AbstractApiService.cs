using HappeningsDotNetC.Helpers;
using HappeningsDotNetC.Infrastructure;
using HappeningsDotNetC.Interfaces.EntityInterfaces;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using HappeningsDotNetC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Services
{
    public abstract class AbstractApiService<TEnt, TDto> : IApiEntityService<TEnt, TDto> where TEnt : class, IDbEntity
                                                                                         where TDto : class, IDbDto
    {
        // if you were expecting the Repository pattern see :
        // https://medium.com/@hoagsie/youre-all-doing-entity-framework-wrong-ea0c40e20502
        // https://www.thereformedprogrammer.net/is-the-repository-pattern-useful-with-entity-framework-core/

        protected readonly HappeningsContext happeningsContext;
        protected readonly ILoginService loginService;
        protected readonly Guid? currentUserId;

        public AbstractApiService(HappeningsContext hc, ILoginService loginServ)
        {
            // There is no gaurantee or check that HappeningsContext.Set<TEnt> will actually work
            // later revisions may change this (or even go back to the respository pattern kicking and screaming, if need be)
            happeningsContext = hc;
            loginService = loginServ;
            currentUserId = loginService.GetCurrentUserId();
        }     
        
        protected SystemData SystemData { get { return happeningsContext.SystemData; } }

        public abstract TEnt CreateEntity(TDto dto); // Note this does NOT imply SaveChanges is run and it should NOT be

        protected abstract void UpdateEntity(TEnt entity, TDto dto); // Note this does NOT imply SaveChanges is run and it should NOT be

        public abstract TDto DtoFromEntity(TEnt entity);

        protected virtual void PreCreate(TDto dto) { }
        protected virtual void PostCreate(TEnt ent) { }

        protected virtual void PreUpdate(TDto dto) { }
        protected virtual void PostUpdate(TEnt ent) { }

        protected virtual void PreDelete(TEnt ent) { }
        protected virtual void PostDelete(TEnt ent) { }


        public virtual IEnumerable<TDto> GetForUser(Guid id)
        {
            throw new HandledException(new NotImplementedException("This service doesn't implement GetForUser"));
        }

        #region Encapsulate most DbSet access as the means of DatabaseAccess is a frequent refactoring target

        // this gets overridden by children with any extra includes as needed
        public virtual IQueryable<TEnt> GetQueryable(bool local = false)
        {
            if (local)
            {
                return happeningsContext.Set<TEnt>().Local.AsQueryable();
            }
            else
            {
                return happeningsContext.Set<TEnt>();
            }
        }

        public virtual void DeleteEnt(TEnt ent)
        {
            happeningsContext.Set<TEnt>().Remove(ent);
        }

        protected virtual void AddEnt(TEnt ent)
        {
            happeningsContext.Set<TEnt>().Add(ent);
        }

        //  some services may want to override with pre/post commit behaviors
        protected virtual void SaveChanges()
        {
            happeningsContext.SaveChanges();
        }

        public virtual TEnt GetEnt(Guid id)
        {
            var entResult = GetEntOrDefault(id);

            if (entResult == null) { throw new HandledException(new KeyNotFoundException()); }

            return entResult;
        }

        public virtual TEnt GetEntOrDefault(Guid id)
        {
            return GetQueryable().SingleOrDefault(x => x.Id == id);
        }

        public virtual bool Exists(Guid id)
        {
            // not using GetEnt here because exception catching is slower
            var entResult = GetQueryable().SingleOrDefault(x => x.Id == id);

            return entResult != null;
        }

        #endregion        

        protected TDto CreateCore(TDto dto, bool commitImmediately = true, bool skipValidation = false)
        {
            // validation skipping provides for cases where validation is done elsewhere. External consumers can't disable this
            if (!skipValidation) PreCreate(dto);

            TEnt newEnt = CreateEntity(dto);
            AddEnt(newEnt);

            if (commitImmediately)
            {
                SaveChanges();
            }

            if (!skipValidation) PostCreate(newEnt);

            return DtoFromEntity(newEnt);
        }

        public virtual TDto Create(TDto dto, bool commitImmediately = true)
        {
            return CreateCore(dto, commitImmediately);            
        }

        public virtual IEnumerable<TDto> Create(IEnumerable<TDto> dtos, bool commitImmediately = true)
        {
            List<TDto> result = new List<TDto>(dtos.Count());

            foreach(TDto dto in dtos)
            {
                result.Add(Create(dto, false));
            }

            if (commitImmediately)
            {
                SaveChanges();
            }

            return result;
        }

        public IEnumerable<TDto> Get()
        {
            return GetQueryable().Select(x => DtoFromEntity(x));
        }

        public int GetCount()
        {
            return GetQueryable().Count();
        }

        public TDto Get(Guid id)
        {
            TEnt foundEntity = GetEnt(id);
            return DtoFromEntity(foundEntity);
        }

        protected TDto UpdateCore(TDto dto, bool commitImmediately = true, bool skipValidation = false)
        {
            // validation skipping provides for cases where validation is done elsewhere. External consumers can't disable this
            if (!skipValidation) PreUpdate(dto);

            TEnt foundEntity = GetEnt(dto.Id);
            UpdateEntity(foundEntity, dto);

            if (!skipValidation) PostUpdate(foundEntity);

            if (commitImmediately)
            {
                SaveChanges();
            }

            return DtoFromEntity(foundEntity);
        }

        public virtual TDto Update(TDto dto, bool commitImmediately = true)
        {
            return UpdateCore(dto, commitImmediately);
        }

        protected void DeleteCore(Guid id, bool commitImmediately = true, bool skipValidation = false)
        {
            TEnt foundEntity = GetEnt(id);
            if (!skipValidation) PreDelete(foundEntity);

            DeleteEnt(foundEntity);

            if (!skipValidation) PostDelete(foundEntity);

            if (commitImmediately)
            {
                SaveChanges();
            }
        }

        public virtual void Delete(Guid id, bool commitImmediately = true)
        {
            DeleteCore(id, commitImmediately, false);
        }

        public virtual IEnumerable<TDto> Update(IEnumerable<TDto> dtos, bool commitImmediately = true)
        {
            List<TDto> result = new List<TDto>(dtos.Count());

            foreach (TDto dto in dtos)
            {
                result.Add(Update(dto, false));
            }

            if (commitImmediately)
            {
                SaveChanges();
            }

            return result;
        }

        public virtual void Delete(IEnumerable<Guid> ids, bool commitImmediately = true)
        {
            // this approach is not the most efficient but is easy code to read and the application has no requirements to perform at large scales
            foreach (Guid id in ids)
            {
                Delete (id, false);
            }

            if (commitImmediately)
            {
                SaveChanges();
            }

            // the idea of returning a number of entities deleted was considered but since any ids not found generates an error, 
            // the result will always either be an error thrown or the full count of ids deleted
        }
    }
}
