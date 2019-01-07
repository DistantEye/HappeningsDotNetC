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
                ApiRegister(userInfo);
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
        [HttpPost]
        public async Task<bool> ApiLogin(LoginDto loginDto)
        {

            return await loginService.Login(loginDto);
            
        }

        [HttpGet]
        public IActionResult ApiLogout()
        {
            loginService.Logout();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public UserDto ApiRegister(UserDto userInfo)
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

            return userService.Create(userInfo.CloneWithNewInfo(preregisterInfo));

        }

    }
}
