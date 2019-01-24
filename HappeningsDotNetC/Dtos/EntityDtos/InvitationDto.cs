using HappeningsDotNetC.Interfaces.EntityInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Dtos.EntityDtos
{
    public class InvitationDto : IDbDto
    {
        public Guid Id { get; set; }
        public Guid HappeningId { get; set; }
        public string HappeningName { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public int ReminderXMinsBefore { get; set; }
        public bool IsPrivate { get; set; }

        // extra happening fields to save on lookups
        public string HappeningDesc { get; set; }
        public string HappeningControllingUser { get; set; }
        public Guid HappeningControllingUserId { get; set; }
        public DateTime EndDate { get; set; }

    }
}
