using HappeningsDotNetC.Interfaces.EntityInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Interfaces.ServiceInterfaces
{
    // It's good to hide Entity level access from TDto level access, especially when the below DtoFromEntity may one day become an aspect of a different service
    public interface IApiEntityService<TEnt, TDto> : IApiService<TDto> where TEnt : class, IDbEntity
                                                                        where TDto : class, IDbDto
    {
        IQueryable<TEnt> GetQueryable();
        TEnt GetEnt(Guid id);
        TEnt GetEntOrDefault(Guid id);
        TDto DtoFromEntity(TEnt entity);

        TEnt CreateEntity(TDto dto); // used in some rare cases for service<->service interactions
        void DeleteEnt(TEnt ent); // same justification
    }
}
