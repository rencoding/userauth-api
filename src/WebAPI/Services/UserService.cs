using System;
using System.Threading.Tasks;

using AutoMapper;

using GenesisTest.DAL.Repository;
using GenesisTest.WebAPI.Helpers;
using GenesisTest.WebAPI.DomainEntities;
using DbUser = GenesisTest.DAL.Models.User;
using DomainUser = GenesisTest.WebAPI.DomainEntities.User;

namespace GenesisTest.WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        private readonly ITokenManager tokenManager;

        private const int TokenExpiryInMinutes = 30;

        private IMapper mapper;
        public UserService(IUserRepository userRepository, ITokenManager tokenManager, IMapper mapper)
        {
            this.userRepository = userRepository;

            this.tokenManager = tokenManager;

            this.mapper = mapper;
        }

        /// <summary>
        /// Sign up the user. If email exists, then return error
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> SignUp(ref DomainUser user)
        {
            DbUser dbUser = userRepository.InsertUser(mapper.Map<DbUser>(user));

            if (dbUser != null)
            {
                // Update Token
                dbUser.Token = tokenManager.GenerateTokenForUser(dbUser.Id.ToString());
                dbUser.Password = BCrypt.Net.BCrypt.HashPassword(dbUser.Password);
                userRepository.UpdateUser(dbUser);

                // Map
                user.Id = dbUser.Id;
                user.Token = dbUser.Token;
                user.CreatedOn = dbUser.CreatedOn;
                user.LastLoginOn = dbUser.LastLoginOn;
                user.LastUpdatedOn = dbUser.LastUpdatedOn;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Sign in by Email and Password
        /// </summary>
        /// <param name="submittedEmail">User submitted Email</param>
        /// <param name="submittedPassword">User submitted Password</param>
        /// <param name="User"></param>
        /// <returns></returns>
        public Task<bool> SignIn(string submittedEmail, string submittedPassword, out DomainEntities.User User)
        {
            User = null;
            DbUser dbUser = userRepository.FindUserByEmail(submittedEmail);
            if (dbUser != null)
            {
                bool validPassword = BCrypt.Net.BCrypt.Verify(submittedPassword, dbUser.Password);

                if (validPassword)
                {
                    // Update Token, Last updatedon, Last loginon
                    dbUser.Token = tokenManager.GenerateTokenForUser(dbUser.Id.ToString());
                    dbUser = userRepository.UpdateUser(dbUser);
                    User = new DomainEntities.User();
                    User.Id = dbUser.Id;
                    User.Token = dbUser.Token;
                    User.CreatedOn = dbUser.CreatedOn;
                    User.LastLoginOn = dbUser.LastLoginOn;
                    User.LastUpdatedOn = dbUser.LastUpdatedOn;

                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Authenticate UserSession
        /// </summary>
        /// <param name="submittedUserId"></param>
        /// <param name="jwtToken"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<UserSessionStatus> AuthenticateSession(Guid submittedUserId, string jwtToken, out DomainUser user)
        {
            user = new DomainEntities.User();

            DbUser dbUser = userRepository.FindUserById(submittedUserId);

            if (dbUser != null)
            {
                if (dbUser.Token == jwtToken)
                {
                    TimeSpan timeElapsed = DateTime.UtcNow - dbUser.LastLoginOn;
                    if (timeElapsed.TotalMinutes < TokenExpiryInMinutes)
                    {
                        user.Id = dbUser.Id;
                        user.Token = dbUser.Token;
                        user.CreatedOn = dbUser.CreatedOn;
                        user.LastLoginOn = dbUser.LastLoginOn;
                        user.LastUpdatedOn = dbUser.LastUpdatedOn;

                        return Task.FromResult(UserSessionStatus.Authorized);
                    }
                    else
                    {
                        return Task.FromResult(UserSessionStatus.InvalidSession);
                    }
                }
                else
                {
                    return Task.FromResult(UserSessionStatus.Unauthorized);
                }
            }
            return Task.FromResult(UserSessionStatus.Unauthorized);
        }
    }
}
