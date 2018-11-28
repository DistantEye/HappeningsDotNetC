using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Entities.JoinEntities
{
    // since EF Core as of time of writing doesn't support implicitly defined Many to Many relationships
    public class EventUser
    {
        public Guid Id { get; set; }

        [ForeignKey("Event")]
        public Guid EventId { get; set; }
        public Event Event { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
