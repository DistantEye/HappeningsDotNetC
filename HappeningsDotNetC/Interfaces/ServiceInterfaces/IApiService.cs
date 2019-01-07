using HappeningsDotNetC.Interfaces.EntityInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Interfaces.ServiceInterfaces
{

    public interface IApiService<TDto> where TDto : class, IDbDto
    {
        IEnumerable<TDto> Get();
        TDto Get(Guid id);
        
        TDto Create(TDto dto, bool commitImmediately = true);
        TDto Update(TDto dto, bool commitImmediately = true);
        void Delete(Guid id, bool commitImmediately = true); // no bool needed, it should throw an Error directly if there's any problems deleting

        IEnumerable<TDto> Create(IEnumerable<TDto> dtos, bool commitImmediately = true);
        IEnumerable<TDto> Update(IEnumerable<TDto> dtos, bool commitImmediately = true);
        void Delete(IEnumerable<Guid> ids, bool commitImmediately = true); // no bool needed, it should throw an Error directly if there's any problems deleting

        bool Exists(Guid id); // quick shortcut for making sure an Entity exists (or doesn't exist, if trying to avoid duplicates)

        IEnumerable<TDto> GetForUser(Guid id); // not necessarily implemented for all things. tries to return user specific info
    }    

}
