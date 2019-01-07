using HappeningsDotNetC.Models.JoinEntities;
using HappeningsDotNetC.Interfaces.EntityInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Models
{
    public class Happening : IDbEntity
    {
        public Guid Id { get; protected set; }

        public string Name { get; protected set; }
        public string Description { get; protected set; }

        // Note that this User does not have to be a member of AllUsers
        public User ControllingUser { get; protected set; }
        public Guid ControllingUserId { get; protected set; }

        
        public ICollection<HappeningUser> AllUsers { get; protected set; }        

        public DateTime StartTime { get; protected set; }
        public DateTime EndTime { get; protected set; }

        public bool IsPrivate { get; protected set; }

        protected Happening()
        {

        }

        public Happening(Guid id, string name, string description, Guid controllingUserId, DateTime startTime, DateTime endTime, bool isPrivate)
        {
            Id = id;
            AllUsers = new List<HappeningUser>();
            Update(name, description, controllingUserId, startTime, endTime, isPrivate);
        }

        public void Update(string name, string description, Guid controllingUserId, DateTime startTime, DateTime endTime, bool isPrivate)
        {
            Name = name;
            Description = description;
            ControllingUserId = controllingUserId;
            StartTime = startTime;
            EndTime = endTime;
            IsPrivate = isPrivate;
        }
    }

    public enum RSVP
    {
        NoResponse, Yes, No, Maybe
    }
}
