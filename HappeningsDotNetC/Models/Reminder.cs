using HappeningsDotNetC.Models.JoinEntities;
using HappeningsDotNetC.Interfaces.EntityInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HappeningsDotNetC.Models
{
    public class Reminder : IDbEntity
    {
        public Guid Id { get; protected set; }

        public DateTime StartRemindAt { get; protected set; }
        public DateTime HappeningTime { get; protected set; }

        public bool IsSilenced { get; protected set; } 
        
        public HappeningUser HappeningUser { get; protected set; }
        [Required]
        public Guid? HappeningUserId { get; protected set; }

        protected Reminder()
        {

        }

        public Reminder(Guid id, DateTime startRemindAt, DateTime happeningTime, bool isSilenced, Guid? happeningUserId)
        {
            Id = id;
            HappeningUserId = happeningUserId;
            Update(startRemindAt, happeningTime, isSilenced);
        }

        public void Update(DateTime startRemindAt, DateTime happeningTime, bool isSilenced)
        {
            StartRemindAt = startRemindAt;
            HappeningTime = happeningTime;
            IsSilenced = isSilenced;
        }
    }
}
