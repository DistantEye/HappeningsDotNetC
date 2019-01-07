using HappeningsDotNetC.Interfaces.EntityInterfaces;
using HappeningsDotNetC.Dtos.IntermediaryDtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Dtos.EntityDtos
{
    public class UserDto : IDbDto
    {   

        public Guid Id { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public bool CalendarVisibleToOthers { get; set; }

        public IEnumerable<Guid> Happenings { get; set; }

        // this should only be sent in during Registration related service communication, never returned/mapped, obviously
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PasswordOrHash { get; set; }

        public UserDto()
        {
            CalendarVisibleToOthers = true; // default value
        }

        public UserDto CloneWithNewInfo(CreatedLoginDto loginInfo)
        {
            return new UserDto()
            {
                Id = this.Id,
                Role = this.Role,
                Name = loginInfo.UserName,
                FriendlyName = this.FriendlyName,
                CalendarVisibleToOthers = this.CalendarVisibleToOthers,
                Happenings = this.Happenings,
                PasswordOrHash = loginInfo.Hash
            };
        }
    }
}
