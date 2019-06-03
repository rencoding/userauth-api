using System.Collections.Generic;
using System.Linq;

using AutoMapper;
using Xunit;

using GenesisTest.WebAPI.AutoMapper;
using DbUser = GenesisTest.DAL.Models.User;
using DomainUser = GenesisTest.WebAPI.DomainEntities.User;

namespace GenesisTest.UnitTest.WebAPI
{
    public class AutomapperUserProfileShould
    {
        [Fact]
        public void MapDomainUser_to_ModelUser_Test()
        {
            // Arrange
            var userProfile = new UserAutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(userProfile));
            IMapper mapper = new Mapper(configuration);
            DomainUser domainUser = new DomainUser()
            {
                Name = "NewUser",
                Email = "new-email@test.com",
                Password = "new-password",
                TelephoneNumbers = new List<string> { "1234567890", "4412345678" }
            };

            // Act
            var mappedDbUser = mapper.Map<DbUser>(domainUser);

            // Assert
            Assert.Equal(domainUser.Name, mappedDbUser.Name);
            Assert.Equal(domainUser.Email, mappedDbUser.Email);
            Assert.Equal(domainUser.Password, mappedDbUser.Password);
            Assert.Equal(domainUser.TelephoneNumbers.Count(), mappedDbUser.UserTelephones.Count());
            Assert.Equal(domainUser.TelephoneNumbers.First(), mappedDbUser.UserTelephones[0].TelephoneNumber);
        }
    }
}
