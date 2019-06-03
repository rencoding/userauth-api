using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using GenesisTest.DAL.Models;
using DbUser = GenesisTest.DAL.Models.User;

namespace IntegrationTest
{
    public class CustomWebApplicationFactory<TStartup> :
        WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create new Service provider
                var serviceProvider = new ServiceCollection()
                 .AddEntityFrameworkInMemoryDatabase()
                 .BuildServiceProvider();

                // Add UserDbContext for testing purpose
                services.AddDbContext<UserDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryUsersDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to to obtain reference to the test UsersDb
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<UserDbContext>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
                        db.Users.AddRange(GetFakeUsersData());
                        db.SaveChanges();

                    }
                    catch
                    {
                        throw; 
                    }
                }
            });
        }

        private IEnumerable<DbUser> GetFakeUsersData()
        {
            List<DbUser> testUsers = new List<DbUser>();
            Random random = new Random();
            int howManyTestUsers = random.Next(1, 15);

            for (int i = 0; i < howManyTestUsers; i++)
            {
                Guid userId = Guid.NewGuid();
                testUsers.Add(new DbUser()
                {
                    Id = userId,
                    Name = $"TestUser{i}",
                    Email = $"TestUser{i}@email.com",
                    Password = BCrypt.Net.BCrypt.HashPassword(@"""$%^!"),
                    Token =  "",
                    CreatedOn = DateTime.UtcNow,
                    LastLoginOn = DateTime.UtcNow,
                    LastUpdatedOn = DateTime.UtcNow,
                    UserTelephones = new List<UserTelephone>() { new UserTelephone { TelephoneNumber = "1234567890", UserId = userId } }
                });
            }
            return testUsers;
        }
    }
}
