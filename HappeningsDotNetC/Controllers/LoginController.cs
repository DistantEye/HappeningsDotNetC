using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HappeningsDotNetC.Models;
using HappeningsDotNetC.Dtos.EntityDtos;
using HappeningsDotNetC.Dtos.IntermediaryDtos;
using HappeningsDotNetC.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using HappeningsDotNetC.Helpers;

namespace HappeningsDotNetC.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService loginService;
        private readonly IApiService<UserDto> userService;

        public LoginController(ILoginService lS, IApiService<UserDto> uS) : base()
        {
            loginService = lS;
            userService = uS;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string message = "")
        {
            ViewData["Title"] = "Login";
            ViewData["Message"] = message;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {

            if (await ApiLogin(loginDto))
            {
                return new RedirectToActionResult("Index", "Calendar", new { });
            }
            else
            {
                return new RedirectToActionResult("Login", "Login", new { message = "Username or password is incorrect" });
            }

        }

        [HttpGet]
        public IActionResult Logout()
        {
            ApiLogout();
            return new RedirectToActionResult("Login", "Login", new { });
        }

        [AllowAnonymous]
        public IActionResult Register(string message = "")
        {
            ViewData["Message"] = message;            
            
            return View(new UserDto());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(UserDto userInfo)
        {
            LoginDto smallerDto = new LoginDto()
            {
                UserName = userInfo.Name,
                Password = userInfo.PasswordOrHash
            };

            try
            {
                await ApiRegister(userInfo);
            }
            catch(ArgumentException ae)
            {
                // user creation was rejected
                return new RedirectToActionResult("Register", "Login", new { message = ae });
            }

            // login user once successfully registered
            return await Login(smallerDto);
        }

        [AllowAnonymous]
        [HttpPost("/api/[controller]/login")]
        public async Task<bool> ApiLogin([FromBody] LoginDto loginDto)
        {
            try
            {
                return await loginService.Login(loginDto);
            }
            catch (HandledException e)
            {
                if (e.InnerException is KeyNotFoundException)
                {
                    return false; // we want to hide "user not found" errors
                }
                else
                {
                    throw e;
                }
            }
            
        }

        [HttpGet("/api/[controller]/logout")]
        public IActionResult ApiLogout()
        {
            loginService.Logout();
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("/api/[controller]/getCurrentUser")]
        public UserDto ApiGetCurrentUser()
        {
            var currentUser = loginService.GetCurrentUser();

            if (currentUser == null) { return null; }

            return userService.Get(currentUser.Id);
        }

        [AllowAnonymous]
        [HttpPost("/api/[controller]/register")]
        public async Task<UserDto> ApiRegister([FromBody] UserDto userInfo)
        {
            // if no users, the first one made is always admin
            if (userService.Get().Count() == 0)
            {
                userInfo.Role = "Admin";
            }
            else
            {
                userInfo.Role = "Normal"; 
            }

            LoginDto smallerDto = new LoginDto()
            {
                UserName = userInfo.Name,
                Password = userInfo.PasswordOrHash
            };
            CreatedLoginDto preregisterInfo = loginService.RegisterOrUpdate(smallerDto); // this is just hashing the password and should always succeed

            var result = userService.Create(userInfo.CloneWithNewInfo(preregisterInfo)); 

            // register implies Login(user) if no one is currently logged in

            if (ApiGetCurrentUser() == null)
            {
                await ApiLogin(smallerDto);
            }

            return result;

        }

    }
}
