using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using GenesisTest.DAL.Repository;
using GenesisTest.DAL.Models;
using GenesisTest.WebAPI.Helpers;
using GenesisTest.WebAPI.Services;

using DbUser = GenesisTest.DAL.Models.User;
using DbUserTelephone = GenesisTest.DAL.Models.UserTelephone;
using DomainUser = GenesisTest.WebAPI.DomainEntities.User;
using GenesisTest.WebAPI.DomainEntities;

namespace GenesisTest.UnitTest.WebAPI
{
    public class UserServiceShould
    {

        private Mock<IUserRepository> userRepository;
        private Mock<IOptions<AppSettings>> appSettings;
        private Mock<IMapper> mapper;
        private Mock<ITokenManager> tokenManager;
        private IUserService userService_UnderTest;

        private const int TokenExpiryInMinutes = 30;

        public UserServiceShould()
        {
            // Arrange
            userRepository = new Mock<IUserRepository>();
            appSettings = GetMockedAppSettings();
            mapper = new Mock<IMapper>();
            tokenManager = new Mock<ITokenManager>();
            tokenManager.Setup(t => t.GenerateTokenForUser(It.IsAny<string>())).Returns(It.IsAny<string>());
        }

        [Fact]
        public void FailAnExistingUserFromSignupAgain_Test()
        {
            // Arrange
            DomainUser anExistingUser = new DomainUser()
            {
                Name = "ExistingTestUser",
                Email = "existing-email@test.com",
                Password = "existing-password"
            };
            mapper.Setup(x => x.Map<DomainUser, DbUser>(anExistingUser)).Returns((DbUser)null);
            userService_UnderTest = new UserService(userRepository.Object, tokenManager.Object, mapper.Object);
            userRepository.Setup(r => r.InsertUser(It.IsAny<DbUser>())).Returns((DbUser)null);

            // Act
            var result = userService_UnderTest.SignUp(ref anExistingUser).Result;

            // Assert 
            Assert.False(result);
            userRepository.Verify(x => x.InsertUser(It.IsAny<DbUser>()), Times.Once()); // To verify that Insert() called once
            userRepository.Verify(x => x.UpdateUser(It.IsAny<DbUser>()), Times.Never()); // To verify that Update() never called for an existing user
        }

        [Fact]
        public void SignUpNewUser_Test()
        {
            // Arrange
            DomainUser newUser = new DomainUser()
            {
                Name = "NewUser",
                Email = "new-email@test.com",
                Password = "new-password",
                TelephoneNumbers = new List<string> { "1234567890", "4412345678" }
            };
            DbUser dbNewUser = new DbUser() { Password = BCrypt.Net.BCrypt.HashPassword("new-password") };
            mapper.Setup(r => r.Map<DbUser>(It.IsAny<DomainUser>())).Returns(dbNewUser);
            userService_UnderTest = new UserService(userRepository.Object, tokenManager.Object, mapper.Object);
            
            // Act
            userRepository.Setup(r => r.InsertUser(It.IsAny<DbUser>())).Returns(dbNewUser);
            userRepository.Setup(r => r.UpdateUser(It.IsAny<DbUser>())).Returns(dbNewUser);
            var result = userService_UnderTest.SignUp(ref newUser).Result;

            // Assert 
            Assert.True(result);
            userRepository.Verify(x => x.InsertUser(It.IsAny<DbUser>()), Times.Once()); // To verify that Insert() called once
            userRepository.Verify(x => x.UpdateUser(It.IsAny<DbUser>()), Times.Once()); // To verify that Update() never called for an existing user
        }

        [Fact]
        public void InvalidateSignIn_WrongEmail_Test()
        {
            // Arrange
            string invalidEmail = "invalid-email@test.com";
            string invalidPassword = "invalid-password";
            userService_UnderTest = new UserService(userRepository.Object, tokenManager.Object, mapper.Object);

            // Act
            userRepository.Setup(x => x.FindUserByEmail(invalidEmail)).Returns((DbUser)null);

            // Assert
            var result = userService_UnderTest.SignIn(invalidEmail, invalidPassword, out DomainUser user).Result;
            Assert.False(result);
            Assert.Null(user);
        }

        [Fact]
        public void InvalidateSignIn_WrongPassword_Test()
        {
            // Arrange
            string validEmail = "valid-email@test.com";
            string invalidPassword = "invalid-password";
            userService_UnderTest = new UserService(userRepository.Object, tokenManager.Object, mapper.Object);

            // Act
            userRepository.Setup(x => x.FindUserByEmail(validEmail)).Returns(new DbUser { Password = "$2a$11$8lNytkap2lGflznmZM6O.OLFetId/pPCE2a4Ugpgj5dxWZKisuG02" });

            // Assert
            var result = userService_UnderTest.SignIn(validEmail, invalidPassword, out DomainUser user).Result;
            Assert.False(result);
            Assert.Null(user);
        }

        [Fact]
        public void SignIn_ValidUser_Test()
        {
            // Arrange
            string validEmail = "valid-email@test.com";
            string validPassword = "12345";
            userService_UnderTest = new UserService(userRepository.Object, tokenManager.Object, mapper.Object);
            userRepository.Setup(x => x.FindUserByEmail(validEmail)).Returns(new DbUser() { Password = "$2a$11$8lNytkap2lGflznmZM6O.OLFetId/pPCE2a4Ugpgj5dxWZKisuG02" });
            userRepository.Setup(x => x.UpdateUser(It.IsAny<DbUser>())).Returns(new DbUser() { });

            // Act
            var result = userService_UnderTest.SignIn(validEmail, validPassword, out DomainUser user).Result;

            // Assert
            Assert.True(result);
            Assert.NotNull(user);
        }

        [Fact]
        public void AuthenticateSession_InvalidUserId_Test()
        {
            // Arrange
            string jwtToken = "";
            Guid userId = Guid.NewGuid();
            userRepository.Setup(x => x.FindUserById(userId)).Returns((DbUser)null);
            userService_UnderTest = new UserService(userRepository.Object, tokenManager.Object, mapper.Object);

            // Act
            UserSessionStatus userStatus = userService_UnderTest.AuthenticateSession(userId, jwtToken, out DomainUser user).Result;

            // Assert
            Assert.Equal(UserSessionStatus.Unauthorized, userStatus);
        }

        [Fact]
        public void AuthenticateSession_InvalidToken_Test()
        {
            // Arrange
            string jwtToken = "Invalid-Token";
            Guid userId = Guid.NewGuid();
            userRepository.Setup(x => x.FindUserById(userId)).Returns(
                new DbUser()
                {
                    Id = userId,
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjZiNDYzZjVlLTZlNDItNDU3Mi04OGMxLTQxODE3MTQ3NDc5MSIsIm5iZiI6MTU1ODk0MTkzMCwiZXhwIjoxNTU4OTQzNzMwLCJpYXQiOjE1NTg5NDE5MzB9.46Zr8u8QA0aL2ujrRvZM0eLXrkgzvRXtXACoQQcx1I4"
                });
            userService_UnderTest = new UserService(userRepository.Object, tokenManager.Object, mapper.Object);

            // Act
            UserSessionStatus userStatus = userService_UnderTest.AuthenticateSession(userId, jwtToken, out DomainUser user).Result;

            // Assert
            Assert.Equal(UserSessionStatus.Unauthorized, userStatus);

        }

        [Fact]
        public void AuthenticateSession_UserAndTokenAreValid_Test()
        {
            // Arrange
            string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjZiNDYzZjVlLTZlNDItNDU3Mi04OGMxLTQxODE3MTQ3NDc5MSIsIm5iZiI6MTU1ODk0MTkzMCwiZXhwIjoxNTU4OTQzNzMwLCJpYXQiOjE1NTg5NDE5MzB9.46Zr8u8QA0aL2ujrRvZM0eLXrkgzvRXtXACoQQcx1I4";
            Guid userId = Guid.NewGuid();
            userRepository.Setup(x => x.FindUserById(userId)).Returns(
                new DbUser()
                {
                    Id = userId,
                    Token = jwtToken,
                    LastLoginOn = DateTime.UtcNow.AddMinutes(-1 * new Random().Next(0, TokenExpiryInMinutes))
                });
            userService_UnderTest = new UserService(userRepository.Object, tokenManager.Object, mapper.Object);

            // Act
            UserSessionStatus userStatus = userService_UnderTest.AuthenticateSession(userId, jwtToken, out DomainUser user).Result;

            // Assert
            Assert.Equal(UserSessionStatus.Authorized, userStatus);
        }

        [Fact]
        public void AuthenticateSession_UserValidButTokenExpired_Test()
        {
            // Arrange
            string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjZiNDYzZjVlLTZlNDItNDU3Mi04OGMxLTQxODE3MTQ3NDc5MSIsIm5iZiI6MTU1ODk0MTkzMCwiZXhwIjoxNTU4OTQzNzMwLCJpYXQiOjE1NTg5NDE5MzB9.46Zr8u8QA0aL2ujrRvZM0eLXrkgzvRXtXACoQQcx1I4";
            Guid userId = Guid.NewGuid();
            userRepository.Setup(x => x.FindUserById(userId)).Returns(
                new DbUser()
                {
                    Id = userId,
                    Token = jwtToken,
                    LastLoginOn = DateTime.UtcNow.AddMinutes(-1 * TokenExpiryInMinutes)
                });
            userService_UnderTest = new UserService(userRepository.Object, tokenManager.Object, mapper.Object);

            // Act
            UserSessionStatus userStatus = userService_UnderTest.AuthenticateSession(userId, jwtToken, out DomainUser user).Result;

            // Assert
            Assert.Equal(UserSessionStatus.InvalidSession, userStatus);
        }

        private Mock<IOptions<AppSettings>> GetMockedAppSettings()
        {
            AppSettings app = new AppSettings() { Secret = "TEST - THIS IS USED TO SIGN AND VERIFY JWT TOKENS IN GenesisTest" };
            var mock = new Mock<IOptions<AppSettings>>();
            mock.Setup(ap => ap.Value).Returns(app);
            return mock;
        }
    }
}
