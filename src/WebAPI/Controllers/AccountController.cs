using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

using GenesisTest.WebAPI.DomainEntities;
using GenesisTest.WebAPI.Services;

namespace GenesisTest.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService userService;

        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// POST: api/account/signup
        /// </summary>
        /// <param name="User"></param>
        /// <returns>A new user is created if email is not already registered. If not, return false</returns>
        [AllowAnonymous]
        [HttpPost("Signup", Name = "Signup")]
        public async Task<IActionResult> SignUp(User User)
        {
            if (await userService.SignUp(ref User))
            {
                return Ok(new
                {
                    id = User.Id,
                    createdOn = User.CreatedOn,
                    lastUpdatedOn = User.LastUpdatedOn,
                    lastLoginOn = User.LastLoginOn,
                    token = User.Token
                });
            }
            else
            {
                return Ok(new { message = "E-mail already exists" });
            }
        }

        /// <summary>
        /// Sign-in  
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <returns>Upon successful Sign-in, it returns the new User including new token</returns>
        [AllowAnonymous]
        [HttpGet("Signin", Name = "Signin")]
        public async Task<IActionResult> SignIn(string Email, string Password)
        {
            if (await userService.SignIn(Email, Password, out User User))
            {
                return Ok(new
                {
                    id = User.Id,
                    createdOn = User.CreatedOn,
                    lastUpdatedOn = User.LastUpdatedOn,
                    lastLoginOn = User.LastLoginOn,
                    token = User.Token
                });
            }
            return Ok(new { message = "Invalid user and / or password" });
        }

        /// <summary>
        /// Search for User
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Returns User after validating the User Id and Token</returns>
        [HttpGet("SearchUser/{Id}", Name = "SearchUser")]
        public IActionResult SearchUser(Guid Id)
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues bearerToken);
            string token = bearerToken.ToString().Replace("Bearer", "").Trim();

            UserSessionStatus userStatus = userService.AuthenticateSession(Id, token, out User user).Result;

            switch (userStatus)
            {
                case UserSessionStatus.Authorized:
                    return Ok(new
                    {
                        id = user.Id,
                        createdOn = user.CreatedOn,
                        lastUpdatedOn = user.LastUpdatedOn,
                        lastLoginOn = user.LastLoginOn,
                        token = user.Token
                    });

                case UserSessionStatus.Unauthorized:
                    {
                        return Ok(new { message = "Unauthorized" });
                    }
                case UserSessionStatus.InvalidSession:
                    {
                        return Ok(new { message = "Invalid Session" });
                    }
            }
            return Ok(new { message = "Error" });
        }

    }
}
