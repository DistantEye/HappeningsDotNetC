using HappeningsDotNetC.Entities.JoinEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Entities
{
    public class User
    {   
        public Guid Id { get; protected set; }
        public UserRole Role { get; protected set; }
        public string Name { get; protected set; }
        public string FriendlyName { get; protected set; }
        public bool CalendarVisibleToOthers { get; protected set; }

        public ICollection<EventUser> Events { get; protected set; }

        public User()
        {

        }

        public User(Guid id, UserRole role, string name, string friendlyName, bool calendarVisibleToOthers)
        {
            Id = id;
            Role = role;
            Name = name;
            FriendlyName = friendlyName;
            CalendarVisibleToOthers = calendarVisibleToOthers;
            Events = new List<EventUser>();
        }

        public void Update(UserRole role, string name, string friendlyName, bool calendarVisibleToOthers)
        {
            Role = role;
            Name = name;
            FriendlyName = friendlyName;
            CalendarVisibleToOthers = calendarVisibleToOthers;
        }
    }

    public enum UserRole
    {
        Normal, Admin
    }
}
