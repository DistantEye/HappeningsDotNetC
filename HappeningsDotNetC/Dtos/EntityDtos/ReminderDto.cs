using HappeningsDotNetC.Interfaces.EntityInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Dtos.EntityDtos
{
    public class ReminderDto : IDbDto
    {

        public Guid Id { get; set; }

        public DateTime StartRemindAt { get; set; }
        public DateTime HappeningTime { get; set; }

        public Guid HappeningId { get; set; }
        public string HappeningName { get; set; }

        public bool IsSilenced { get; set; }

        public Guid UserId { get; set; }

    }
}
