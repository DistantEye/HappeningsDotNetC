using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Interfaces.EntityInterfaces
{
    // common interface for all directly database tied Entities, this is sparse now but is for future proofing
    public interface IDbEntity
    {
        Guid Id { get; }
    }
}
