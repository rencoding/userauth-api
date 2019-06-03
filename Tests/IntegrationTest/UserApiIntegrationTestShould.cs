using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using DomainUser = GenesisTest.WebAPI.DomainEntities.User;
using System;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace IntegrationTest
{
    public class UserApiTestShould : IClassFixture<CustomWebApplicationFactory<GenesisTest.WebAPI.Startup>>
    {
        private readonly CustomWebApplicationFactory<GenesisTest.WebAPI.Startup> factory;

        public UserApiTestShould(CustomWebApplicationFactory<GenesisTest.WebAPI.Startup> factory)
        {
            this.factory = factory;
        }

        private const string expectedCotentType = "application/json";

        [Theory]
        [InlineData("api/account/signup")]
        public async Task SignUpNewUser_Test(string url)
        {
            // Arrange
            var client = factory.CreateClient();

            DomainUser newUser = new DomainUser()
            {
                Name = "NewUser",
                Email = "new-email@test.com",
                Password = "new-password",
                TelephoneNumbers = new List<string> { "1234567890", "4412345678" }
            };

            // Act 
            var response = await client.PostAsJsonAsync(url, newUser);
            string result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCotentType, response.Content.Headers.ContentType.MediaType);
            Assert.Contains("id", result);
            Assert.Contains("createdOn", result);
            Assert.Contains("lastUpdatedOn", result);
            Assert.Contains("lastLoginOn", result);
            Assert.Contains("token", result);
        }

        [Theory]
        [InlineData("api/account/signup")]
        public async Task FailAnExistingUserFromSignupAgain_Test(string url)
        {
            // Arrange
            var client = factory.CreateClient();
            DomainUser newUser = new DomainUser()
            {
                Name = "ExistingTestUser",
                Email = "TestUser0@email.com", // An existing email-id from the seed
                Password = "existing-password",
                TelephoneNumbers = new List<string> { "1234567890", "4412345678" }
            };

            // Act 
            var response = await client.PostAsJsonAsync(url, newUser);
            string result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCotentType, response.Content.Headers.ContentType.MediaType);
            Assert.Contains("E-mail already exists", result);
        }

        [Theory]
        [InlineData("api/account/signin?email={email}&password={password}")]
        public async Task SignIn_ValidUser_Test(string url)
        {
            // Arrange
            var client = factory.CreateClient();

            // Act 
            var response = await client.GetAsync(url.Replace("{email}", "TestUser0@email.com").Replace("{password}", @"""$%^!"));
            string result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCotentType, response.Content.Headers.ContentType.MediaType);
            Assert.Contains("id",result);
            Assert.Contains("createdOn", result);
            Assert.Contains("lastUpdatedOn", result);
            Assert.Contains("lastLoginOn", result);
            Assert.Contains("token", result);
        }

        [Theory]
        [InlineData("api/account/signin?email={email}&password={password}")]
        public async Task InvalidateSignIn_WrongEmail_Test(string url)
        {
            // Arrange
            var client = factory.CreateClient();

            // Act 
            var response = await client.GetAsync(url.Replace("{email}", "invalid-email@test.com").Replace("{password}", "invalid-password"));
            string result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedCotentType, response.Content.Headers.ContentType.MediaType);
            Assert.Contains("Invalid user and / or password", result);
        }

        [Theory]
        [InlineData("api/account/searchuser/{id}")]
        public async Task AuthenticateSession_NoToken_Test(string url)
        {
            // Arrange
            var client = factory.CreateClient();

            // Act 
            var response = await client.GetAsync(url.Replace("{id}", Guid.NewGuid().ToString()));
            string result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Contains("Unauthorized", result);
        }

        [Theory]
        [InlineData("api/account/searchuser/{id}")]
        public async Task AuthenticateSession_InvalidId_Test(string url)
        {

            // Arrange: Sign-in a user and get Id and Token; Pollute that for testing Invalid Id
            var client = factory.CreateClient();
            const string signUrl = "api/account/signin?email={email}&password={password}";
            var response = await client.GetAsync(signUrl.Replace("{email}", "TestUser0@email.com").Replace("{password}", @"""$%^!"));
            string result = await response.Content.ReadAsStringAsync();
            JObject jUserObj = JObject.Parse(result);
            DomainUser signedInUser = jUserObj.ToObject<DomainUser>();
            
            // Act
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", signedInUser.Token);
            response = await client.GetAsync(url.Replace("{id}", Guid.NewGuid().ToString()));
            result = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Contains("Unauthorized", result);
        }

        [Theory]
        [InlineData("api/account/searchuser/{id}")]
        public async Task AuthenticateSession_ValidUser_Test(string url)
        {
            // Arrange: Sign-in a user and get Id and Token; Pollute that for testing Invalid Id
            var client = factory.CreateClient();
            const string signUrl = "api/account/signin?email={email}&password={password}";
            var response = await client.GetAsync(signUrl.Replace("{email}", "TestUser0@email.com").Replace("{password}", @"""$%^!"));
            string result = await response.Content.ReadAsStringAsync();
            JObject jUserObj = JObject.Parse(result);
            DomainUser signedInUser = jUserObj.ToObject<DomainUser>();
            
            // Act
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", signedInUser.Token);
            response = await client.GetAsync(url.Replace("{id}", signedInUser.Id.ToString()));
            result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("id", result);
            Assert.Contains("createdOn", result);
            Assert.Contains("lastUpdatedOn", result);
            Assert.Contains("lastLoginOn", result);
            Assert.Contains("token", result);
        }
    }
}
