using HappeningsDotNetC.Interfaces.EntityInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Models.JoinEntities
{
    // since EF Core as of time of writing doesn't support implicitly defined Many to Many relationships, we have an explict bridge entity
    // and since it had to be here anyways, some non-bridge relationship-scoped data fields were added
    public class HappeningUser : IDbEntity
    {
        public Guid Id { get; protected set; }

        [ForeignKey("Happening")]
        public Guid HappeningId { get; protected set; }
        public Happening Happening { get; protected set; }

        [ForeignKey("User")]
        public Guid UserId { get; protected set; }
        public User User { get; protected set; }

        public RSVP UserStatus { get; protected set; }
        public int ReminderXMinsBefore { get; protected set; }

        public bool IsPrivate { get; set; }

        [ForeignKey("Reminder")]
        [Required]
        public Guid? ReminderId { get; protected set; }
        public Reminder Reminder { get; protected set; }

        protected HappeningUser()
        {
        }

        public HappeningUser(Guid id, Guid happeningId, Guid userId, RSVP userStatus, int reminderXMinsBefore, bool isPrivate)
        {
            Id = id;
            HappeningId = happeningId;
            UserId = userId;
            Update(userStatus, reminderXMinsBefore, isPrivate);
        }

        public void Update(RSVP userStatus, int reminderXMinsBefore, bool isPrivate)
        {
            UserStatus = userStatus;
            ReminderXMinsBefore = reminderXMinsBefore;
            IsPrivate = isPrivate;
        }

        // reminder is currently a tightly bound dependent object but that could one day change. a reminder that seemingly meaningless mutators like this
        // still serve a purpose
        public void SetReminder(Reminder r)
        {
            Reminder = r;
        }
    }
}
