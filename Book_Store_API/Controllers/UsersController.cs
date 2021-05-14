using Book_Store_API.Contract;
using Book_Store_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Store_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> signInManager_;
        private readonly UserManager<IdentityUser> userManager_;
        private readonly ILoggerService loggerService_;
        public UsersController(SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, ILoggerService loggerService)
        {
            signInManager_ = signInManager;
            userManager_ = userManager;
            loggerService_ = loggerService;
        }
        /// <summary>
        /// User Login endpoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var username = userDTO.UserName;
            var pwd = userDTO.Password;
            loggerService_.LogInfo($"Login called for {username}");
            var result = await signInManager_.PasswordSignInAsync(username, pwd, false, false);
            if(result.Succeeded)
            {
                loggerService_.LogInfo($"{username} Authenticated Successfully!");
                var user = await userManager_.FindByNameAsync(username);
                return Ok();
            }
            loggerService_.LogError($"Authentication Failed for {userDTO}");
            return Unauthorized(userDTO);
        }
    }
}
