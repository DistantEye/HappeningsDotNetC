using HappeningsDotNetC.Interfaces.EntityInterfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Dtos.EntityDtos
{
    public class HappeningDto : IDbDto
    {

        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Flavor { get; set; }

        public string ControllingUser { get; set; }
        public Guid ControllingUserId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool IsPrivate { get; set; }

        // Ignore if null
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Guid> AllUsers { get; set; }

        // Ignore if null - optionally can pull the current user's invitation (if it exists)
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public InvitationDto CurrentUserInfo { get; set; }

        // Ignore if null - this is a more expensive grab only done on demand
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<InvitationDto> AllUserInfo { get; set; }

        // Ignore if null - this is for whatever current User might be interacting with the event
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserStatus { get; set; }
    }
}
