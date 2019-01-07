using HappeningsDotNetC.Models.JoinEntities;
using HappeningsDotNetC.Interfaces.EntityInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Models
{
    public class User : IDbEntity
    {   
        public Guid Id { get; protected set; }
        public UserRole Role { get; protected set; }
        public string Name { get; protected set; } // what binds to Login
        public string FriendlyName { get; protected set; }
        public bool CalendarVisibleToOthers { get; protected set; }

        public string Hash { get; protected set; }

        public ICollection<HappeningUser> Happenings { get; protected set; }


        public User()
        {

        }

        public User(Guid id, UserRole role, string name, string friendlyName, bool calendarVisibleToOthers, string hash = null)
        {
            Id = id;            
            Happenings = new List<HappeningUser>();

            Update(role, name, friendlyName, calendarVisibleToOthers, hash);
        }

        public void Update(UserRole role, string name, string friendlyName, bool calendarVisibleToOthers, string hash = null)
        {
            Role = role;
            Name = name;
            FriendlyName = friendlyName;
            CalendarVisibleToOthers = calendarVisibleToOthers;

            if (hash != null) Hash = hash;
        }
    }

    public enum UserRole
    {
        Normal, Admin
    }
}
