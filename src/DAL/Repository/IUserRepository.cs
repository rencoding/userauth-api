using System;
using GenesisTest.DAL.Models;

namespace GenesisTest.DAL.Repository
{
    public interface IUserRepository
    {
        /// <summary>
        /// Insert User into Db
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        User InsertUser(User user);

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="user"></param>
        User UpdateUser(User user);

        /// <summary>
        /// Find User by Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        User FindUserByEmail(string email);

        /// <summary>
        /// Find User by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        User FindUserById(Guid Id);
    }
}
