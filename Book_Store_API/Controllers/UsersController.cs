using Book_Store_API.Contract;
using Book_Store_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        private readonly IConfiguration config_;
        public UsersController(SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, ILoggerService loggerService,
            IConfiguration config)
        {
            signInManager_ = signInManager;
            userManager_ = userManager;
            loggerService_ = loggerService;
            config_ = config;
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
                var tokenString = await GenerateJSONWebToken(user);
                return Ok(new { token = tokenString });
            }
            loggerService_.LogError($"Authentication Failed for {userDTO}");
            return Unauthorized(userDTO);
        }
        private async Task<string> GenerateJSONWebToken(IdentityUser identityUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config_["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id)
            };
            var roles = await userManager_.GetRolesAsync(identityUser);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));
            var token = new JwtSecurityToken(config_["Jwt:Issuer"],
                config_["Jwt:Issuer"],
                claims,
                null,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
