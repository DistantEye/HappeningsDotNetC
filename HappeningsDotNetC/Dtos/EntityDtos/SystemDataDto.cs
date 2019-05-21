using HappeningsDotNetC.Interfaces.EntityInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Dtos.EntityDtos
{
    public class SystemDataDto : IDbDto
    {
        public Guid Id { get; set; }
        public bool OpenRegistration { get; set; }
    }
}
