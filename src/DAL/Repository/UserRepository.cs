using System;
using System.Linq;
using GenesisTest.DAL.Models;

namespace GenesisTest.DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext userDbContext;

        public UserRepository(UserDbContext context)
        {
            userDbContext = context;
        }

        /// <summary>
        /// Insert User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User InsertUser(User user)
        {
            if (userDbContext.Users.Where(x => x.Email == user.Email).Count() == 0)
            {
                user.Id = Guid.NewGuid();
                user.CreatedOn = DateTime.UtcNow;
                user.LastLoginOn = user.CreatedOn;
                userDbContext.Users.Add(user);
                userDbContext.SaveChanges();
                return user;
            }
            return null;
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="user"></param>
        public User UpdateUser(User user)
        {
            user.LastLoginOn = DateTime.UtcNow;
            user.LastUpdatedOn = DateTime.UtcNow;

            userDbContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            userDbContext.SaveChanges();

            return user;
        }

        /// <summary>
        /// Find User by Email, if found, it should be only one User having the given email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User FindUserByEmail(string email)
        {
            return userDbContext.Users.SingleOrDefault(x => x.Email == email);
        }

        /// <summary>
        /// Find User by Id, if found, it should be only one User having the given Id 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public User FindUserById(Guid Id)
        {
            return userDbContext.Users.SingleOrDefault(x => x.Id == Id);
        }
    }
}
