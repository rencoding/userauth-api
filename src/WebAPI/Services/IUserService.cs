using System;
using System.Threading.Tasks;
using GenesisTest.WebAPI.DomainEntities;

namespace GenesisTest.WebAPI.Services
{
    public interface IUserService
    {
        /// <summary>
        /// /// Sign up the user. If email exists, then return error
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> SignUp(ref User user);

        /// <summary>
        /// Sign in by Email and Password
        /// </summary>
        /// <param name="submittedEmail"></param>
        /// <param name="submittedPassword"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        Task<bool> SignIn(string submittedEmail, string submittedPassword, out User User);

        /// <summary>
        /// Authenticate Session
        /// </summary>
        /// <param name="submittedUserId"></param>
        /// <param name="jwtToken"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<UserSessionStatus> AuthenticateSession(Guid submittedUserId, string jwtToken, out User user);
    }
}
