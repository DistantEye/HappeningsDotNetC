using HappeningsDotNetC.Entities.JoinEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Entities
{
    public class Event
    {
        public Guid Id { get; protected set; }

        public DateTime FriendlyName { get; protected set; }
        public string Description { get; protected set; }

        public User PrimaryUser { get; protected set; }
        public Guid PrimaryUserId { get; protected set; }

        public ICollection<EventUser> AllUsers { get; protected set; }


    }
}
