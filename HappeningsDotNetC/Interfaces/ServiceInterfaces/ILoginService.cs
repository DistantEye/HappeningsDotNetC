using HappeningsDotNetC.Models;
using HappeningsDotNetC.Interfaces.EntityInterfaces;
using HappeningsDotNetC.Dtos.IntermediaryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Interfaces.ServiceInterfaces
{
    public interface ILoginService
    {
        Task<bool> Login(LoginDto dto);
        void Logout();
        CreatedLoginDto RegisterOrUpdate(LoginDto dto);
        IEnumerable<CreatedLoginDto> RegisterOrUpdate(IEnumerable<LoginDto> dtos);
        Guid? GetCurrentUserId(bool errorOnNull = false);
        User GetCurrentUser();
        User FindUser(string userName);
        bool IsLoginSane();
    }
}
