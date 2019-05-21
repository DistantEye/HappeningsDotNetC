using HappeningsDotNetC.Dtos.EntityDtos;

namespace HappeningsDotNetC.Dtos.IntermediaryDtos
{
    public class SystemInfoDto
    {

        public UserDto CurrentUser { get; set; }
        public int ReminderCount { get; set; }
        public bool HasUsers { get; set; }
        public bool OpenRegistration { get; set; }

    }
}
